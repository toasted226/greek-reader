namespace GreekReader.Interop

type PartOfSpeech =
    | Noun = 0
    | Verb = 1
    | Adjective = 2
    | Adverb = 3
    | Determiner = 4
    | Pronoun = 5
    | Preposition = 6
    | Conjunction = 7
    | Interjection = 8
    | Particle = 9
    | Numeral = 10
    | ProperNoun = 11
    | Article = 12

[<CLIMutable>]
type Example = { Greek: string; English: string }

// Features is the grammatical path in mood, voice, aspect, tense, person, number order,
// e.g. [| "indicative"; "active"; "imperfective"; "past"; "first-person"; "singular" |].
// Article forms carry "definite" or "indefinite" as their leading feature.
[<CLIMutable>]
type FormEntry = { Form: string; Features: string[] }

[<CLIMutable>]
type LemmaEntry =
    { Lemma: string
      Pos: PartOfSpeech
      Glosses: string[]
      Examples: Example[]
      Forms: FormEntry[] }
