namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module NounParser =

  /// Build a lookup map from (case, number) to the form string.
  /// Uses first occurrence if duplicates exist.
  let private buildFormMap (forms: Form list) : Map<string * string, string> =
    forms
    |> List.choose (fun f ->
      match f.Tags with
      | None -> None
      | Some tags ->
        match Utils.pickCase None tags, Utils.pickNumber None tags with
        | Some c, Some n -> Some((c, n), f.Form)
        | _ -> None)
    |> Map.ofList

  let getNounForms (forms: Form list) : CaseForms =
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

  let getGlosses (senses: Sense list) : string list =
    senses
    |> List.map (fun s -> s.Glosses |> Option.defaultValue [])
    |> List.concat
    |> List.distinct

  let analyse (w: Word) : AnalysedWord =
    Noun
      { Lemma = w.Word
        Glosses = w.Senses |> Option.defaultValue [] |> getGlosses
        Gender = w.Senses |> Option.bind getGender
        Forms = w.Forms |> Option.defaultValue [] |> getNounForms }

  let analyseWords (words: Word seq) : AnalysedWord seq =
    words |> Seq.map analyse

  let analyseWordsList (words: Word list) : AnalysedWord list =
    words |> Seq.map analyse |> Seq.toList