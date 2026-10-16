namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module PronounParser =

    let analyse (w: Word) : AnalysedWord =
        let forms = w.Forms |> Option.defaultValue []
        let senses = w.Senses |> Option.defaultValue []

        Pronoun
            { Lemma = w.Word
              Glosses = senses |> Utils.getGlosses
              Declensions =
                match forms |> Utils.formsHaveTags [ "masculine"; "feminine"; "neuter" ] with
                | true -> forms |> Utils.getGender |> GenderDeclined
                | false -> forms |> Utils.getCases |> NonGenderDeclined
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
