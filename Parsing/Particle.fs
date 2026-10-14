namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module ParticleParser =
    
    let analyse (w: Word) : AnalysedWord =
        let senses = w.Senses |> Option.defaultValue []
        Particle
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
