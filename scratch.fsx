#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Pronoun.fs"

open GreekReader.Model
open GreekReader.Parsing.PronounParser
open GreekReader.Wiktionary
open Jsonl

#time "on"
let words = parseLines<Word> ("Resources/greek-pronouns.jsonl") |> Seq.toList
let analysed = analyseWordsList words

let filtered =
    analysed
    |> List.choose (function
        | Pronoun({ Declensions = NonGenderDeclined { Nominative = {Singular = Some _ } } } as p) -> Some(p)
        | _ -> None)
