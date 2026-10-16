namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module AdverbParser =
    
    let getAdverbDegrees (forms: Form list) : AdverbDegrees =
        { Comparative = forms |> Utils.withTag "comparative" |> Utils.getForm
          Superlative = forms |> Utils.withTag "superlative" |> Utils.getForm }
    
    let analyse (w: Word) : AnalysedWord =
        let senses = w.Senses |> Option.defaultValue []
        Adverb
            { Lemma = w.Word
              Glosses = senses |> Utils.getGlosses
              Degrees = w.Forms |> Option.defaultValue [] |> getAdverbDegrees
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
