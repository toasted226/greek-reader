namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module NounParser =

  let private hasTags (case: string, number: string) (tags: string list) =
    tags |> List.contains case && tags |> List.contains number

  /// Build a lookup map from (case, number) to the form string. Uses first
  /// occurrence if duplicates exist (preserves existing semantics for now).
  let private buildFormMap (forms: Form list) : Map<string * string, string> =
    forms
    |> List.choose (fun f ->
      match f.Tags with
      | None -> None
      | Some tags ->
        // try to find the first case/number pair on these tags
        // we only care about the 4 cases + singular/plural
        let rec pickCase acc tags =
          match tags with
          | [] -> None
          | "nominative" :: _ when Option.isNone acc -> pickCase (Some("nominative")) tags
          | "accusative" :: _ when Option.isNone acc -> pickCase (Some("accusative")) tags
          | "genitive" :: _ when Option.isNone acc -> pickCase (Some("genitive")) tags
          | "vocative" :: _ when Option.isNone acc -> pickCase (Some("vocative")) tags
          | _ :: rest -> pickCase acc rest
        let rec pickNumber acc tags =
          match tags with
          | [] -> None
          | "singular" :: _ when Option.isNone acc -> pickNumber (Some("singular")) tags
          | "plural" :: _ when Option.isNone acc -> pickNumber (Some("plural")) tags
          | _ :: rest -> pickNumber acc rest
        match pickCase None tags, pickNumber None tags with
        | Some c, Some n -> Some((c, n), f.Form)
        | _ -> None)
    |> Map.ofList

  let getNounForms (forms: Form list) : NounForms =
    let formMap = buildFormMap forms
    let get case num = formMap |> Map.tryFind (case, num)
    { Nominative = { Singular = get "nominative" "singular"; Plural = get "nominative" "plural" }
      Accusative = { Singular = get "accusative" "singular"; Plural = get "accusative" "plural" }
      Genitive = { Singular = get "genitive" "singular"; Plural = get "genitive" "plural" }
      Vocative = { Singular = get "vocative" "singular"; Plural = get "vocative" "plural" } }

  let private tryGender = function
    | "masculine" -> Some Masculine
    | "feminine" -> Some Feminine
    | "neuter" -> Some Neuter
    | _ -> None

  let getGender (senses: Sense list) : Gender option =
    senses
    |> List.collect (fun s -> s.Tags |> Option.defaultValue [])
    |> List.tryPick tryGender

  let getGlossesPerSense (senses: Sense list) : string list list =
    senses
    |> List.map (fun s -> s.Glosses |> Option.defaultValue [])

  let analyse (w: Word) : AnalysedWord =
    Noun
      { Lemma = w.Word
        GlossesPerSense = w.Senses |> Option.defaultValue [] |> getGlossesPerSense
        Gender = w.Senses |> Option.bind getGender
        Forms = w.Forms |> Option.defaultValue [] |> getNounForms }