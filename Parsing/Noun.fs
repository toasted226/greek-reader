namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module NounParser =

  let getNumbers (forms: Form list) : NumberForms =
    { Singular = forms |> Utils.withTag "singular" |> Utils.getVariants
      Plural = forms |> Utils.withTag "plural" |> Utils.getVariants }

  let getCaseForms (forms: Form list) : CaseForms =
    { Nominative = forms |> Utils.withTag "nominative" |> getNumbers
      Accusative = forms |> Utils.withTag "accusative" |> getNumbers
      Genitive = forms |> Utils.withTag "genitive" |> getNumbers
      Vocative = forms |> Utils.withTag "vocative" |> getNumbers }

  let private tryGender = function
    | "masculine" -> Some Masculine
    | "feminine" -> Some Feminine
    | "neuter" -> Some Neuter
    | _ -> None

  let getGender (senses: Sense list) : Gender option =
    senses
    |> List.collect (fun s -> s.Tags |> Option.defaultValue [])
    |> List.tryPick tryGender

  let analyse (w: Word) : AnalysedWord =
    let senses = w.Senses |> Option.defaultValue []
    Noun
      { Lemma = w.Word
        Glosses = senses |> Utils.getGlosses
        Gender = w.Senses |> Option.bind getGender
        Declensions = w.Forms |> Option.defaultValue [] |> getCaseForms
        Examples = senses |> Utils.getExamples }

  let analyseWords (words: Word seq) : AnalysedWord seq =
    words |> Seq.map analyse

  let analyseWordsList (words: Word list) : AnalysedWord list =
    words |> Seq.map analyse |> Seq.toList