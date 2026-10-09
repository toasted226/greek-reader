module GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module AdjectiveParser =
    
    let analyse (w: Word) : AnalysedWord = failwith "adjective parsing unimplemented"

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
