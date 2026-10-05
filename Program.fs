namespace GreekReader

open GreekReader.Model
open GreekReader.Parsing
open GreekReader.Wiktionary

module Program =

  let analyseNouns (words: Word list) : AnalysedWord list =
    NounParser.analyseWordsList words

  let analyseNounsSeq (words: Word seq) : AnalysedWord seq =
    NounParser.analyseWords words

  // scratch - lazy with cached materialisation
  let private analysedNounsLazy =
    lazy
      (let path =
         let candidates =
           [ "Resources/greek-nouns.jsonl"
             "greek-nouns.jsonl"
             System.IO.Path.Combine(System.AppContext.BaseDirectory, "greek-nouns.jsonl")
             System.IO.Path.Combine(System.AppContext.BaseDirectory, "Resources", "greek-nouns.jsonl")
             System.IO.Path.Combine(System.AppContext.BaseDirectory, "..", "..", "..", "Resources", "greek-nouns.jsonl") ]
         candidates
         |> List.tryFind System.IO.File.Exists
         |> Option.defaultWith (fun () ->
           failwith ("Cannot find greek-nouns.jsonl. Checked: " + String.concat ", " candidates))
       Jsonl.parseWords path |> NounParser.analyseWords |> Seq.toList)

  let analysedNouns = analysedNounsLazy.Value