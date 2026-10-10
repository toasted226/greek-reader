namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module AdverbParser =
    
    let getForm (forms: Form list) : string option =
        match forms |> List.tryHead |> Option.map _.Form with
        | Some "-" -> None
        | Some s -> Some(s)
        | None -> None
    
    let getAdverbDegrees (forms: Form list) : AdverbDegrees =
        { Comparative = forms |> Utils.withTag "comparative" |> getForm
          Superlative = forms |> Utils.withTag "superlative" |> getForm }
    
    let analyse (w: Word) : AnalysedWord =
        Adverb
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Degrees = w.Forms |> Option.defaultValue [] |> getAdverbDegrees }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
