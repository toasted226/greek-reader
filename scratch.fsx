#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Noun.fs"
#load "Parsing/Determiner.fs"
#load "Program.fs"

open GreekReader.Wiktionary
open Jsonl
open GreekReader.Parsing
open DeterminerParser

let words = parseLines<Word>("greek-determiners.jsonl")
let analysed = analyseWordsList(words |> Seq.toList)