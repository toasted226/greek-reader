namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module NumeralParser =

    let getNumeralCases (forms: Form list) : NumeralCases =
        { Nominative = forms |> Utils.withTag "nominative" |> Utils.getVariants
          Accusative = forms |> Utils.withTag "accusative" |> Utils.getVariants
          Genitive = forms |> Utils.withTag "genitive" |> Utils.getVariants
          Vocative = forms |> Utils.withTag "vocative" |> Utils.getVariants }

    let getNumeralGenders (forms: Form list) : NumeralGenders =
        { Masculine = forms |> Utils.withTag "masculine" |> getNumeralCases
          Feminine = forms |> Utils.withTag "feminine" |> getNumeralCases
          Neuter = forms |> Utils.withTag "neuter" |> getNumeralCases }

    let analyse (w: Word) : AnalysedWord =
        let forms = w.Forms |> Option.defaultValue []
        let senses = w.Senses |> Option.defaultValue []

        Numeral
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Declensions =
                match forms |> Utils.formsHaveTags [ "masculine"; "feminine"; "neuter" ] with
                | true -> forms |> getNumeralGenders |> Some
                | false -> None
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
