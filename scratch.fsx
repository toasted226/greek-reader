#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Preposition.fs"

open GreekReader.Parsing.PrepositionParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let words = parseLines<Word> ("Resources/greek-prepositions.jsonl") |> Seq.toList
let analysed = analyseWordsList words
