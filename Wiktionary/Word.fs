namespace GreekReader.Wiktionary

// Mirrors the shape of the Wiktionary dump. Shared by every part-of-speech parser,
// so keep this file faithful to the source schema and free of domain vocabulary.

type Form =
  { Form: string
    Source: string option
    Tags: string list
    Roman: string option }

type Sense =
  { Tags: string list option
    Glosses: string list option }

type Word =
  { Pos: string
    Forms: Form list
    Word: string
    Senses: Sense list option }