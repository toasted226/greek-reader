namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module AdjectiveParser =

    type Degrees =
        { Positive: Form list
          Comparative: Form list
          Superlative: Form list }

    type Degree =
        | Positive
        | Comparative
        | Superlative

    let emptyDegrees =
        { Positive = []
          Comparative = []
          Superlative = [] }

    let private has (tag: string) (form: Form) : bool =
        form.Tags |> Option.defaultValue [] |> List.contains (tag)

    let private addPositive (acc: Degrees) (form: Form) : Degrees =
        { Positive = acc.Positive @ [ form ]
          Comparative = acc.Comparative
          Superlative = acc.Superlative }

    let private addComparative (acc: Degrees) (form: Form) : Degrees =
        { Positive = acc.Positive
          Comparative = acc.Comparative @ [ form ]
          Superlative = acc.Superlative }

    let private addSuperlative (acc: Degrees) (form: Form) : Degrees =
        { Positive = acc.Positive
          Comparative = acc.Comparative
          Superlative = acc.Superlative @ [ form ] }

    let rec separateDegrees (forms: Form list) (degree: Degree option) (acc: Degrees) : Degrees =
        match degree with
        | Some Positive ->
            match forms with
            | [] -> acc
            | head :: tail ->
                if has "inflection-template" head then
                    separateDegrees tail (Some Comparative) acc
                else
                    head |> addPositive acc |> separateDegrees tail degree
        | Some Comparative ->
            match forms with
            | [] -> acc
            | head :: tail ->
                if has "inflection-template" head then
                    separateDegrees tail (Some Superlative) acc
                else
                    head |> addComparative acc |> separateDegrees tail degree
        | Some Superlative ->
            match forms with
            | [] -> acc
            | head :: tail ->
                if has "inflection-template" head then
                    acc
                else
                    head |> addSuperlative acc |> separateDegrees tail degree
        | None ->
            match forms with
            | [] -> acc
            | head :: tail ->
                if has "inflection-template" head then
                    separateDegrees tail (Some Positive) acc
                else
                    separateDegrees tail None acc

    let getForm (forms: Form list) : string option =
        match forms |> List.tryHead |> Option.map _.Form with
        | Some "-" -> None
        | Some s -> Some(s)
        | None -> None

    let getNumbers (forms: Form list) : NumberForms =
        { Singular = forms |> Utils.withTag "singular" |> getForm
          Plural = forms |> Utils.withTag "plural" |> getForm }
    
    let getCases (forms: Form list) : CaseForms =
        { Nominative = forms |> Utils.withTag "nominative" |> getNumbers
          Accusative = forms |> Utils.withTag "accusative" |> getNumbers
          Genitive = forms |> Utils.withTag "genitive" |> getNumbers
          Vocative = forms |> Utils.withTag "vocative" |> getNumbers }

    let getGender (forms: Form list) : GenderForms =
        { Masculine = forms |> Utils.withTag "masculine" |> getCases
          Feminine = forms |> Utils.withTag "feminine" |> getCases
          Neuter = forms |> Utils.withTag "neuter" |> getCases }

    let getDegrees (forms: Form list) : DegreeForms =
        let split = separateDegrees forms None emptyDegrees

        { Positive = getGender split.Positive
          Comparative = getGender split.Comparative
          Superlative = getGender split.Superlative }

    let analyse (w: Word) : AnalysedWord =
        Adjective
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Degrees = w.Forms |> Option.defaultValue [] |> getDegrees }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
