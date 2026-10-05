namespace GreekReader

open GreekReader.Model
open GreekReader.Parsing
open GreekReader.Wiktionary

module Program =

  let analyseNouns (words: Word list) : AnalysedWord list =
    words |> List.map NounParser.analyse

  // scratch
  let private analysedNouns =
    Jsonl.parseWords "greek-nouns.jsonl"
    |> analyseNouns

  [<EntryPoint>]
  let main _ =
    analysedNouns |> ignore
    0