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

type PersonForms =
  { FirstPerson: NumberForms
    SecondPerson: NumberForms
    ThirdPerson: NumberForms }

type TenseForms =
  { Present: PersonForms
    Imperfect: PersonForms
    Past: PersonForms
    Future: PersonForms
    Progressive: PersonForms }

type AspectForms =
  { Imperfective: TenseForms
    Perfective: TenseForms }

type VoiceForms =
  { Active: AspectForms
    Passive: AspectForms }

type MoodForms =
  { Indicative: VoiceForms
    Imperative: VoiceForms }

type ParticipleTenseForms =
  { Present: string option
    Past: string option
    Perfect: string option }

type ParticipleVoiceForms =
  { Active: ParticipleTenseForms
    Passive: ParticipleTenseForms }

type ParticipleForms =
  { Voice: ParticipleVoiceForms }

type InfinitiveForms =
  { Active: string option
    Passive: string option }

type VerbForms =
  { Moods: MoodForms
    Participles: ParticipleForms
    AoristInfinitives: InfinitiveForms }

type NounInfo =
  { Lemma: string
    Glosses: string list
    Gender: Gender option
    Forms: CaseForms }

type DeterminerInfo =
  { Lemma: string
    Glosses: string list
    Forms: GenderForms }

type VerbInfo =
  { Lemma: string
    Glosses: string list
    Conjugations: VerbForms }
  
type AnalysedWord =
  | Noun of NounInfo
  | Determiner of DeterminerInfo
  | Verb of VerbInfo