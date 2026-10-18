namespace GreekReader.Parsing

open System.Linq
open GreekReader.Model
open GreekReader.Wiktionary

module Utils =

    let rec pickGender acc tags =
        match tags with
        | [] -> None
        | "masculine" :: _ when Option.isNone acc -> Some("masculine")
        | "feminine" :: _ when Option.isNone acc -> Some("feminine")
        | "neuter" :: _ when Option.isNone acc -> Some("neuter")
        | _ :: rest -> pickGender acc rest

    let rec pickCase acc tags =
        match tags with
        | [] -> None
        | "nominative" :: _ when Option.isNone acc -> Some("nominative")
        | "accusative" :: _ when Option.isNone acc -> Some("accusative")
        | "genitive" :: _ when Option.isNone acc -> Some("genitive")
        | "vocative" :: _ when Option.isNone acc -> Some("vocative")
        | _ :: rest -> pickCase acc rest

    let rec pickNumber acc tags =
        match tags with
        | [] -> None
        | "singular" :: _ when Option.isNone acc -> Some("singular")
        | "plural" :: _ when Option.isNone acc -> Some("plural")
        | _ :: rest -> pickNumber acc rest

    let getGlosses (senses: Sense list) : string list =
        senses |> List.choose _.Glosses |> List.concat |> List.distinct

    let toModel (e: Example) : GreekReader.Model.Example =
        { Greek = e.Text
          English = e.Translation }

    let getExamples (senses: Sense list) : GreekReader.Model.Example list =
        senses |> List.choose _.Examples |> List.concat |> List.map toModel

    /// returns true if at least one of the provided tags are
    /// present in one of the given forms
    let formsHaveTags (tags: string list) (forms: Form list) : bool =
        forms
        |> List.exists (fun f ->
            tags
            |> List.exists (fun t -> List.contains t (f.Tags |> Option.defaultValue [])))

    let withTag (tag: string) (forms: Form list) : Form list =
        forms |> List.filter (fun f -> (f.Tags |> Option.defaultValue []).Contains(tag))

    let getVariants (forms: Form list) : Variants =
        forms
        |> List.map _.Form
        |> List.choose (fun f ->
            match f with
            | "-" -> None
            | s -> Some(s))

    let getNumbers (forms: Form list) : NumberForms =
        { Singular = forms |> withTag "singular" |> getVariants
          Plural = forms |> withTag "plural" |> getVariants }

    let getCases (forms: Form list) : CaseForms =
        { Nominative = forms |> withTag "nominative" |> getNumbers
          Accusative = forms |> withTag "accusative" |> getNumbers
          Genitive = forms |> withTag "genitive" |> getNumbers
          Vocative = forms |> withTag "vocative" |> getNumbers }

    let getGender (forms: Form list) : GenderForms =
        { Masculine = forms |> withTag "masculine" |> getCases
          Feminine = forms |> withTag "feminine" |> getCases
          Neuter = forms |> withTag "neuter" |> getCases }
