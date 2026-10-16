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

    let getDegrees (forms: Form list) : DegreeForms =
        let split = separateDegrees forms None emptyDegrees

        { Positive = Utils.getGender split.Positive
          Comparative = Utils.getGender split.Comparative
          Superlative = Utils.getGender split.Superlative }

    let analyse (w: Word) : AnalysedWord =
        let senses = w.Senses |> Option.defaultValue []
        Adjective
            { Lemma = w.Word
              Glosses = senses |> Utils.getGlosses
              Degrees = w.Forms |> Option.defaultValue [] |> getDegrees
              Examples = senses |> Utils.getExamples }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
