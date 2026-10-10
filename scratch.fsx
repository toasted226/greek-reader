#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Adverb.fs"

open GreekReader.Parsing.AdverbParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let word = parseLine<Word>("adverb.json")
let analysed = analyse word