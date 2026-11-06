#r "nuget: FSharp.Collections.ParallelSeq"

#load "Model/Morphology.fs"
#load "Wiktionary/Word.fs"
#load "Wiktionary/Jsonl.fs"
#load "Parsing/Utils.fs"
#load "Parsing/Verb.fs"
#load "Parsing/Adjective.fs"
#load "Parsing/Article.fs"
#load "Parsing/Noun.fs"
#load "Parsing/Determiner.fs"
#load "Parsing/Adverb.fs"
#load "Parsing/Pronoun.fs"
#load "Parsing/Preposition.fs"
#load "Parsing/Conjunction.fs"
#load "Parsing/Interjection.fs"
#load "Parsing/Numeral.fs"
#load "Parsing/ProperNoun.fs"
#load "Parsing/Particle.fs"
#load "Parsing/Contraction.fs"
#load "Parsing/Text.fs"
#load "Service.fs"

#time "on"
open System.Text
open GreekReader.Parsing
open GreekReader.Wiktionary

let analysed = Jsonl.parseWords "Resources/greek-contractions.jsonl" |> ContractionParser.analyseWords |> Seq.toList