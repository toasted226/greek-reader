namespace GreekReader.Parsing

open System.Linq

open GreekReader.Model
open GreekReader.Wiktionary

module NounParser =

  let getNounForm (forms: Form list, case: string, number: string) : string option =
    try
      let f =
        forms
        |> List.find (fun f -> f.Tags.Contains(case) && f.Tags.Contains(number))
      Some f.Form
    with
    | _ -> None

  let getNumberFormsForCase (forms: Form list, case: string) : NumberForms =
    { Singular = getNounForm(forms, case, "singular")
      Plural = getNounForm(forms, case, "plural") }

  let getNounForms (forms: Form list) : NounForms =
    { Nominative = getNumberFormsForCase(forms, "nominative")
      Accusative = getNumberFormsForCase(forms, "accusative")
      Genitive = getNumberFormsForCase(forms, "genitive")
      Vocative = getNumberFormsForCase(forms, "vocative") }

  let private tryGender = function
    | "masculine" -> Some Masculine
    | "feminine" -> Some Feminine
    | "neuter" -> Some Neuter
    | _ -> None

  let getGender (senses: Sense list) : Gender option =
    senses
    |> List.collect (fun s -> s.Tags |> Option.defaultValue [])
    |> List.tryPick tryGender

  let getGlosses (senses: Sense list) : string list =
    senses
    |> List.collect (fun s -> s.Glosses |> Option.defaultValue [])

  let analyse (w: Word) : AnalysedWord =
    Noun
      { Lemma = w.Word
        Glosses = w.Senses |> Option.defaultValue [] |> getGlosses
        Gender = w.Senses |> Option.bind getGender
        Forms = getNounForms w.Forms }