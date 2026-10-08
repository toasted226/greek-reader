namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module DeterminerParser =

  let tagsHaveGender (tags: string list option) (gender: string): bool =
    tags
    |> Option.defaultValue []
    |> List.contains gender

  /// Build a lookup map from (case, number) to the form string for the given gender.
  /// Uses first occurrence if duplicates exist.
  let buildFormMap (forms: Form list) (gender: string) : Map<string * string, string> =
    forms
    |> List.filter (fun f -> tagsHaveGender f.Tags gender)
    |> List.choose (fun f ->
      match f.Tags with
      | None -> None
      | Some tags ->
        match Utils.pickCase None tags, Utils.pickNumber None tags with
        | Some c, Some n -> Some((c, n), f.Form)
        | _ -> None)
    |> Map.ofList

  let getCaseForms (forms: Form list) (gender: string) : CaseForms =
    let formMap = buildFormMap forms gender
    let get case num = formMap |> Map.tryFind (case, num)
    { Nominative = { Singular = get "nominative" "singular"; Plural = get "nominative" "plural" }
      Accusative = { Singular = get "accusative" "singular"; Plural = get "accusative" "plural" }
      Genitive = { Singular = get "genitive" "singular"; Plural = get "genitive" "plural" }
      Vocative = { Singular = get "vocative" "singular"; Plural = get "vocative" "plural" } }
    
  let getGenderForms (forms: Form list) : GenderForms =
    { Masculine = getCaseForms forms "masculine"
      Feminine = getCaseForms forms "feminine"
      Neuter = getCaseForms forms "neuter" }

  let analyse (w: Word) : AnalysedWord =
    Determiner
      { Lemma = w.Word
        Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
        Forms = w.Forms |> Option.defaultValue [] |> getGenderForms }

  let analyseWords (words: Word seq) : AnalysedWord seq =
    words |> Seq.map analyse

  let analyseWordsList (words: Word list) : AnalysedWord list =
    words |> Seq.map analyse |> Seq.toList