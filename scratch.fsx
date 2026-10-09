#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Noun.fs"
#load "Parsing/Determiner.fs"
#load "Parsing/Verbs.fs"
#load "Program.fs"

open GreekReader.Parsing.VerbParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let word = parseLine<Word>("verb.json")
let verb = analyse word