namespace GreekReader

open GreekReader.Model
open GreekReader.Parsing
open GreekReader.Wiktionary

module Program =

  let analyseNouns (words: Word list) : AnalysedWord list =
    words |> List.map NounParser.analyse

  // scratch
  let private analysedNouns =
    let candidates =
      [ "Resources/greek-nouns.jsonl"
        "greek-nouns.jsonl"
        System.IO.Path.Combine(System.AppContext.BaseDirectory, "greek-nouns.jsonl")
        System.IO.Path.Combine(System.AppContext.BaseDirectory, "..", "..", "..", "Resources", "greek-nouns.jsonl") ]
    let path =
      candidates
      |> List.tryFind System.IO.File.Exists
      |> Option.defaultWith (fun () ->
        failwith ("Cannot find greek-nouns.jsonl. Checked: " + String.concat ", " candidates))
    Jsonl.parseWords path
    |> analyseNouns

  [<EntryPoint>]
  let main _ =
    analysedNouns |> ignore
    0