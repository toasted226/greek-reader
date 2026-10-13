namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module ProperNounParser =

    let analyse (w: Word) : AnalysedWord =
        let forms = w.Forms |> Option.defaultValue []
        let hasCases =
            forms
            |> Utils.formsHaveTags [ "nominative"; "accusative"; "genitive"; "vocative" ]

        ProperNoun
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Declensions =
                match hasCases with
                | true -> forms |> Utils.getCases |> Some
                | false -> None }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
