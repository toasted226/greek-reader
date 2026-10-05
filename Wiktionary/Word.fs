namespace GreekReader.Wiktionary

// Mirrors the shape of the Wiktionary dump. Shared by every part-of-speech parser,
// so keep this file faithful to the source schema and free of domain vocabulary.

type Form =
  { Form: string
    Source: string option
    /// Absent in 37 of the dataset's forms. Optional so those bind to None
    /// instead of null, which used to throw inside the form lookup.
    Tags: string list option
    Roman: string option }

type Sense =
  { Tags: string list option
    Glosses: string list option }

type Word =
  { Pos: string
    /// Absent entirely in 8 of the dataset's entries (e.g. "ΔΒΔ", "γεν.").
    Forms: Form list option
    Word: string
    Senses: Sense list option }