#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/ProperNoun.fs"

open GreekReader.Parsing.ProperNounParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let words = parseLines<Word> ("Resources/greek-proper-nouns.jsonl") |> Seq.toList
let analysed = analyseWordsList words
