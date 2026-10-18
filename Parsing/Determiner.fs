namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module DeterminerParser =

  let getNumbers (forms: Form list) : NumberForms =
    { Singular = forms |> Utils.withTag "singular" |> Utils.getVariants
      Plural = forms |> Utils.withTag "plural" |> Utils.getVariants }

  let getCaseForms (forms: Form list) : CaseForms =
    { Nominative = forms |> Utils.withTag "nominative" |> getNumbers
      Accusative = forms |> Utils.withTag "accusative" |> getNumbers
      Genitive = forms |> Utils.withTag "genitive" |> getNumbers
      Vocative = forms |> Utils.withTag "vocative" |> getNumbers }

  let getGenderForms (forms: Form list) : GenderForms =
    { Masculine = forms |> Utils.withTag "masculine" |> getCaseForms
      Feminine = forms |> Utils.withTag "feminine" |> getCaseForms
      Neuter = forms |> Utils.withTag "neuter" |> getCaseForms }

  let analyse (w: Word) : AnalysedWord =
    let senses = w.Senses |> Option.defaultValue []
    Determiner
      { Lemma = w.Word
        Glosses = senses |> Utils.getGlosses
        Declensions = w.Forms |> Option.defaultValue [] |> getGenderForms
        Examples = senses |> Utils.getExamples }

  let analyseWords (words: Word seq) : AnalysedWord seq =
    words |> Seq.map analyse

  let analyseWordsList (words: Word list) : AnalysedWord list =
    words |> Seq.map analyse |> Seq.toList