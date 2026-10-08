namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module VerbParser =

  let getVerbForms (forms: Form list) : VerbForms =
    failwith "unimplemented"

  let analyse (w: Word) : AnalysedWord =
    Verb
      { Lemma = w.Word
        Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
        Conjugations = getVerbForms (w.Forms |> Option.defaultValue []) }

  let analyseWords (words: Word seq) : AnalysedWord seq =
    words |> Seq.map analyse

  let analyseWordsList (words: Word list) : AnalysedWord list =
    words |> Seq.map analyse |> Seq.toList
    