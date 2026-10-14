namespace GreekReader.Interop

open System
open System.Collections.Generic

type Gender =
    | Masculine = 0
    | Feminine = 1
    | Neuter = 2

type GrammaticalNumber =
    | Singular = 0
    | Plural = 1

type GrammaticalCase =
    | Nominative = 0
    | Accusative = 1
    | Genitive = 2
    | Vocative = 3

type ArticleKind =
    | Definite = 0
    | Indefinite = 1

type PartOfSpeech =
    | Noun = 0
    | Determiner = 1
    | Verb = 2
    | Adjective = 3
    | Adverb = 4
    | Article = 5
    | Pronoun = 6
    | Preposition = 7
    | Conjunction = 8
    | Interjection = 9
    | Numeral = 10
    | ProperNoun = 11
    | Particle = 12

[<CLIMutable>]
type Example =
    { Greek: string
      English: string }

[<CLIMutable>]
type NumberForms =
    { Singular: string | null
      Plural: string | null }

[<CLIMutable>]
type CaseForms =
    { Nominative: NumberForms
      Accusative: NumberForms
      Genitive: NumberForms
      Vocative: NumberForms }

[<CLIMutable>]
type GenderForms =
    { Masculine: CaseForms
      Feminine: CaseForms
      Neuter: CaseForms }

[<CLIMutable>]
type PersonForms =
    { FirstPerson: NumberForms
      SecondPerson: NumberForms
      ThirdPerson: NumberForms }

[<CLIMutable>]
type TenseForms =
    { Present: PersonForms
      Imperfect: PersonForms
      Past: PersonForms
      Future: PersonForms
      Dependent: PersonForms }

[<CLIMutable>]
type AspectForms =
    { Imperfective: TenseForms
      Perfective: TenseForms }

[<CLIMutable>]
type VoiceForms =
    { Active: AspectForms
      Passive: AspectForms }

[<CLIMutable>]
type MoodForms =
    { Indicative: VoiceForms
      Imperative: VoiceForms }

[<CLIMutable>]
type ParticipleTenseForms =
    { Present: string | null
      Past: string | null
      Perfect: string | null }

[<CLIMutable>]
type ParticipleVoiceForms =
    { Active: ParticipleTenseForms
      Passive: ParticipleTenseForms }

[<CLIMutable>]
type ParticipleForms =
    { Voice: ParticipleVoiceForms }

[<CLIMutable>]
type InfinitiveForms =
    { Active: string | null
      Passive: string | null }

[<CLIMutable>]
type DegreeForms =
    { Positive: GenderForms
      Comparative: GenderForms
      Superlative: GenderForms }

[<CLIMutable>]
type VerbForms =
    { Moods: MoodForms
      Participles: ParticipleForms
      AoristInfinitives: InfinitiveForms }

[<CLIMutable>]
type AdverbDegrees =
    { Comparative: string | null
      Superlative: string | null }

[<CLIMutable>]
type NumeralCases =
    { Nominative: string | null
      Accusative: string | null
      Genitive: string | null
      Vocative: string | null }

[<CLIMutable>]
type NumeralGenders =
    { Masculine: NumeralCases
      Feminine: NumeralCases
      Neuter: NumeralCases }

[<AbstractClass>]
type AnalysedWord() =
    member val Lemma: string | null = null with get, set
    member val Glosses: IReadOnlyList<string> = null with get, set
    abstract PartOfSpeech: PartOfSpeech with get

type NounWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Noun
    member val Gender: Nullable<Gender> = Nullable<Gender>() with get, set
    member val Declensions: CaseForms | null = null with get, set

type DeterminerWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Determiner
    member val Declensions: GenderForms | null = null with get, set

type VerbWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Verb
    member val Conjugations: VerbForms | null = null with get, set

type AdjectiveWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Adjective
    member val Degrees: DegreeForms | null = null with get, set

type AdverbWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Adverb
    member val Degrees: AdverbDegrees | null = null with get, set

type ArticleWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Article
    member val Kind: ArticleKind = ArticleKind.Definite with get, set
    member val Inflections: GenderForms | null = null with get, set

type PronounWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Pronoun
    member val GenderDeclensions: GenderForms | null = null with get, set
    member val CaseDeclensions: CaseForms | null = null with get, set

type PrepositionWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Preposition
    member val Examples: IReadOnlyList<Example> = null with get, set

type ConjunctionWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Conjunction
    member val Examples: IReadOnlyList<Example> = null with get, set

type InterjectionWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Interjection
    member val Examples: IReadOnlyList<Example> = null with get, set

type NumeralWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Numeral
    member val Declensions: NumeralGenders | null = null with get, set

type ProperNounWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.ProperNoun
    member val Declensions: CaseForms | null = null with get, set

type ParticleWord() =
    inherit AnalysedWord()
    override _.PartOfSpeech = PartOfSpeech.Particle
    member val Examples: IReadOnlyList<Example> = null with get, set
