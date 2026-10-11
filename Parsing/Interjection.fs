namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module InterjectionParser =
    
    let analyse (w: Word) : AnalysedWord =
        let senses = w.Senses |> Option.defaultValue []
        Interjection
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
