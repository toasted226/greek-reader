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
#load "Api.fs"

#time "on"
open GreekReader.API

let results =
    queryText "δεν"