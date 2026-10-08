namespace GreekReader.Model

type Gender = Masculine | Feminine | Neuter
type Number = Singular | Plural
type Case = Nominative | Accusative | Genitive | Vocative

type NumberForms =
  { Singular: string option
    Plural: string option }
  
type CaseForms =
  { Nominative: NumberForms
    Accusative: NumberForms
    Genitive: NumberForms
    Vocative: NumberForms }

type GenderForms =
  { Masculine: CaseForms
    Feminine: CaseForms
    Neuter: CaseForms }

type NounInfo =
  { Lemma: string
    Glosses: string list
    Gender: Gender option
    Forms: CaseForms }

type DeterminerInfo =
  { Lemma: string
    Glosses: string list
    Forms: GenderForms }
  
type AnalysedWord =
  | Noun of NounInfo
  | Determiner of DeterminerInfo