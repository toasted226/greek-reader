#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Noun.fs"
#load "Parsing/Determiner.fs"
#load "Parsing/Verbs.fs"
#load "Program.fs"

open GreekReader.Wiktionary
open Jsonl
open GreekReader.Parsing

let word = parseLine<Word>("verb.json")
let forms = word.Forms |> Option.defaultValue []
let moods = VerbParser.getMoods forms