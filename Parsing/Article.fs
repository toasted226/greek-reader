namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module ArticleParser =
    
    let getForm (forms: Form list) : string option =
        match forms |> List.tryHead |> Option.map _.Form with
        | Some "-" -> None
        | Some s -> Some(s)
        | None -> None
    
    let getNumbers (forms: Form list) : NumberForms =
        { Singular =
            match forms |> Utils.formsHaveTags ["definite"] with
            | true -> forms |> Utils.withTag "singular" |> Utils.getVariants
            | false -> forms |> Utils.getVariants
          Plural = forms |> Utils.withTag "plural" |> Utils.getVariants }
    
    let getCases (forms: Form list) : CaseForms =
        { Nominative = forms |> Utils.withTag "nominative" |> getNumbers
          Accusative = forms |> Utils.withTag "accusative" |> getNumbers
          Genitive = forms |> Utils.withTag "genitive" |> getNumbers
          Vocative = forms |> Utils.withTag "vocative" |> getNumbers }

    let getGender (forms: Form list) : GenderForms =
        { Masculine = forms |> Utils.withTag "masculine" |> getCases
          Feminine = forms |> Utils.withTag "feminine" |> getCases
          Neuter = forms |> Utils.withTag "neuter" |> getCases }
    
    let analyse (w: Word) : AnalysedWord =
        let forms = w.Forms |> Option.defaultValue []
        let senses = w.Senses |> Option.defaultValue []
        
        Article
            { Lemma = w.Word
              Glosses = senses |> Utils.getGlosses
              Type =
                  match forms |> Utils.formsHaveTags ["definite"] with
                  | true -> Definite
                  | false -> Indefinite
              Inflections = forms |> getGender
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
