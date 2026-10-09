#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Adjective.fs"

open GreekReader.Parsing.AdjectiveParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let word = parseLine<Word>("adjective.json")
let analysed = analyse word