namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module NounParser =

  let private hasTags (case: string, number: string) (tags: string list) =
    tags |> List.contains case && tags |> List.contains number

  let getNounForm (forms: Form list, case: string, number: string) : string option =
    forms
    |> List.tryPick (fun f ->
      f.Tags
      |> Option.filter (hasTags(case, number))
      |> Option.map (fun _ -> f.Form))

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

  let getGlossesPerSense (senses: Sense list) : string list list =
    senses
    |> List.map (fun s -> s.Glosses |> Option.defaultValue [])

  let analyse (w: Word) : AnalysedWord =
    Noun
      { Lemma = w.Word
        GlossesPerSense = w.Senses |> Option.defaultValue [] |> getGlossesPerSense
        Gender = w.Senses |> Option.bind getGender
        Forms = w.Forms |> Option.defaultValue [] |> getNounForms }