namespace GreekReader.Model

type Gender =
    | Masculine
    | Feminine
    | Neuter

type Number =
    | Singular
    | Plural

type Case =
    | Nominative
    | Accusative
    | Genitive
    | Vocative

type ArticleType =
    | Definite
    | Indefinite

type Example = { Greek: string; English: string }

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
      Dependent: PersonForms }

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

type ParticipleForms = { Voice: ParticipleVoiceForms }

type InfinitiveForms =
    { Active: string option
      Passive: string option }

type DegreeForms =
    { Positive: GenderForms
      Comparative: GenderForms
      Superlative: GenderForms }

type VerbForms =
    { Moods: MoodForms
      Participles: ParticipleForms
      AoristInfinitives: InfinitiveForms }

type AdverbDegrees =
    { Comparative: string option
      Superlative: string option }

type NounInfo =
    { Lemma: string
      Glosses: string list
      Gender: Gender option
      Declensions: CaseForms
      Examples: Example list }

type DeterminerInfo =
    { Lemma: string
      Glosses: string list
      Declensions: GenderForms
      Examples: Example list }

type VerbInfo =
    { Lemma: string
      Glosses: string list
      Conjugations: VerbForms
      Examples: Example list }

type AdjectiveInfo =
    { Lemma: string
      Glosses: string list
      Degrees: DegreeForms
      Examples: Example list }

type AdverbInfo =
    { Lemma: string
      Glosses: string list
      Degrees: AdverbDegrees
      Examples: Example list }

type ArticleInfo =
    { Lemma: string
      Glosses: string list
      Type: ArticleType
      Inflections: GenderForms
      Examples: Example list }

type PronounDeclensions =
    | GenderDeclined of GenderForms
    | NonGenderDeclined of CaseForms

type PronounInfo =
    { Lemma: string
      Glosses: string list
      Declensions: PronounDeclensions
      Examples: Example list }

type PrepositionInfo =
    { Lemma: string
      Glosses: string list
      Examples: Example list }

type ConjunctionInfo =
    { Lemma: string
      Glosses: string list
      Examples: Example list }

type InterjectionInfo =
    { Lemma: string
      Glosses: string list
      Examples: Example list }

type NumeralCases =
    { Nominative: string option
      Accusative: string option
      Genitive: string option
      Vocative: string option }

type NumeralGenders =
    { Masculine: NumeralCases
      Feminine: NumeralCases
      Neuter: NumeralCases }

type NumeralInfo =
    { Lemma: string
      Glosses: string list
      Declensions: NumeralGenders option
      Examples: Example list }

type ProperNounInfo =
    { Lemma: string
      Glosses: string list
      Declensions: CaseForms option
      Examples: Example list }

type ParticleInfo =
    { Lemma: string
      Glosses: string list
      Examples: Example list }

type AnalysedWord =
    | Noun of NounInfo
    | Determiner of DeterminerInfo
    | Verb of VerbInfo
    | Adjective of AdjectiveInfo
    | Adverb of AdverbInfo
    | Article of ArticleInfo
    | Pronoun of PronounInfo
    | Preposition of PrepositionInfo
    | Conjunction of ConjunctionInfo
    | Interjection of InterjectionInfo
    | Numeral of NumeralInfo
    | ProperNoun of ProperNounInfo
    | Particle of ParticleInfo
