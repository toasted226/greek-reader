namespace GreekReader.Model

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
    GlossesPerSense: string list list
    Gender: Gender option
    Forms: NounForms }