#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Article.fs"

open GreekReader.Parsing.ArticleParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let words = parseLines<Word>("Resources/greek-articles.jsonl") |> Seq.toList
let analysed = analyseWordsList words