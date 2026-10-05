open System.IO
open System.Linq
open System.Text.Json

type Case = Nominative | Accusative | Genitive | Vocative
type Number = Singular | Plural
type Gender = Masculine | Feminine | Neuter

type NumberForms =
  { Singular: string option
    Plural: string option }
  
type NounForms =
  { Nominative: NumberForms
    Accusative: NumberForms
    Genitive: NumberForms
    Vocative: NumberForms }

type NounInfo =
  { Lemma: string
    Glosses: string list
    Gender: Gender option
    Romanized: string option
    Forms: NounForms }

type AnalysedWord =
  | Noun of NounInfo
  
// json parse struct
type Form =
  { Form: string
    Source: string option
    Tags: string list
    Roman: string option }

// json parse struct
type Sense =
  { Tags: string list option
    Glosses: string list option }

// json parse struct
type Word =
  { Pos: string
    Forms: Form list
    Word: string
    Senses: Sense list option }

let getNounForm(forms: Form list, case: string, number: string): string option =
  try 
    let f = forms
            |> List.find (fun f -> f.Tags.Contains(case) && f.Tags.Contains(number))
    Some f.Form
  with
  | _ -> None

let getNumberFormsForCase (forms: Form list, case: string): NumberForms =
  { Singular = getNounForm(forms, case, "singular")
    Plural = getNounForm(forms, case, "plural") }
  
let getNounForms (forms: Form list): NounForms =
  { Nominative = getNumberFormsForCase(forms, "nominative")
    Accusative = getNumberFormsForCase(forms, "accusative")
    Genitive = getNumberFormsForCase(forms, "genitive")
    Vocative = getNumberFormsForCase(forms, "vocative") }

let tryGender = function
  | "masculine" -> Some Masculine
  | "feminine" -> Some Feminine
  | "neuter" -> Some Neuter
  | _ -> None

let getGender (senses: Sense list): Gender option =
  senses
  |> List.collect (fun s -> s.Tags |> Option.defaultValue [])
  |> List.tryPick tryGender

let getGlosses (senses: Sense list): string list =
  senses
  |> List.collect (fun s -> s.Glosses |> Option.defaultValue [])

let analyseNoun (w: Word): AnalysedWord =
  Noun
    { Lemma = w.Word
      Glosses = w.Senses |> Option.defaultValue [] |> getGlosses
      Gender = w.Senses |> Option.bind getGender
      Forms = getNounForms w.Forms }

let rec analyseNouns (words: Word list): AnalysedWord list =
  words
  |> List.map analyseNoun

let parseNounsJson (filepath: string) =
  let lines = File.ReadLines filepath
  let options = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
  lines
  |> Seq.map (fun l -> JsonSerializer.Deserialize<Word>(l, options))
  |> Seq.toList

// scratch
let analysedNouns =
  parseNounsJson "greek-nouns.jsonl"
  |> analyseNouns