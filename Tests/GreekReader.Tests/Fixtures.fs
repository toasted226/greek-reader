module GreekReader.Tests.Fixtures

open GreekReader.Model

let ex g e : Example = { Greek = g; English = e }

let nf (singular: string option) (plural: string option) : NumberForms =
    { Singular = singular; Plural = plural }

let noCases () : CaseForms =
    { Nominative = nf None None
      Accusative = nf None None
      Genitive = nf None None
      Vocative = nf None None }

let cases nominative accusative genitive vocative : CaseForms =
    { Nominative = nominative
      Accusative = accusative
      Genitive = genitive
      Vocative = vocative }

let noGenderForms () : GenderForms =
    { Masculine = noCases ()
      Feminine = noCases ()
      Neuter = noCases () }

let genderForms masculine feminine neuter : GenderForms =
    { Masculine = masculine
      Feminine = feminine
      Neuter = neuter }

let noDegreeForms () : DegreeForms =
    { Positive = noGenderForms ()
      Comparative = noGenderForms ()
      Superlative = noGenderForms () }

let degreeForms positive comparative superlative : DegreeForms =
    { Positive = positive
      Comparative = comparative
      Superlative = superlative }

let noun lemma glosses gender declensions examples : NounInfo =
    { Lemma = lemma
      Glosses = glosses
      Gender = gender
      Declensions = declensions
      Examples = examples }

let article lemma glosses kind inflections examples : ArticleInfo =
    { Lemma = lemma
      Glosses = glosses
      Type = kind
      Inflections = inflections
      Examples = examples }

let adjective lemma glosses degrees examples : AdjectiveInfo =
    { Lemma = lemma
      Glosses = glosses
      Degrees = degrees
      Examples = examples }

let determiner lemma glosses declensions examples : DeterminerInfo =
    { Lemma = lemma
      Glosses = glosses
      Declensions = declensions
      Examples = examples }

let pronoun lemma glosses declensions examples : PronounInfo =
    { Lemma = lemma
      Glosses = glosses
      Declensions = declensions
      Examples = examples }

let preposition lemma glosses examples : PrepositionInfo =
    { Lemma = lemma
      Glosses = glosses
      Examples = examples }

let conjunction lemma glosses examples : ConjunctionInfo =
    { Lemma = lemma
      Glosses = glosses
      Examples = examples }

let adverbDegrees comparative superlative : AdverbDegrees =
    { Comparative = comparative
      Superlative = superlative }

let adverb lemma glosses degrees examples : AdverbInfo =
    { Lemma = lemma
      Glosses = glosses
      Degrees = degrees
      Examples = examples }

let interjection lemma glosses examples : InterjectionInfo =
    { Lemma = lemma
      Glosses = glosses
      Examples = examples }

let numeralCases nominative accusative genitive vocative : NumeralCases =
    { Nominative = nominative
      Accusative = accusative
      Genitive = genitive
      Vocative = vocative }

let noNumeralGenders () : NumeralGenders =
    { Masculine = numeralCases None None None None
      Feminine = numeralCases None None None None
      Neuter = numeralCases None None None None }

let numeralGenders masculine feminine neuter : NumeralGenders =
    { Masculine = masculine
      Feminine = feminine
      Neuter = neuter }

let numeral lemma glosses declensions examples : NumeralInfo =
    { Lemma = lemma
      Glosses = glosses
      Declensions = declensions
      Examples = examples }

let properNoun lemma glosses declensions examples : ProperNounInfo =
    { Lemma = lemma
      Glosses = glosses
      Declensions = declensions
      Examples = examples }

let particle lemma glosses examples : ParticleInfo =
    { Lemma = lemma
      Glosses = glosses
      Examples = examples }

let personForms first second third : PersonForms =
    { FirstPerson = first
      SecondPerson = second
      ThirdPerson = third }

let emptyPersonForms () : PersonForms =
    { FirstPerson = nf None None
      SecondPerson = nf None None
      ThirdPerson = nf None None }

let tenseForms present imperfect past future dependent : TenseForms =
    { Present = present
      Imperfect = imperfect
      Past = past
      Future = future
      Dependent = dependent }

let emptyTenseForms () : TenseForms =
    { Present = emptyPersonForms ()
      Imperfect = emptyPersonForms ()
      Past = emptyPersonForms ()
      Future = emptyPersonForms ()
      Dependent = emptyPersonForms () }

let aspectForms imperfective perfective : AspectForms =
    { Imperfective = imperfective
      Perfective = perfective }

let emptyAspectForms () : AspectForms =
    { Imperfective = emptyTenseForms ()
      Perfective = emptyTenseForms () }

let voiceForms active passive : VoiceForms =
    { Active = active
      Passive = passive }

let emptyVoiceForms () : VoiceForms =
    { Active = emptyAspectForms ()
      Passive = emptyAspectForms () }

let moodForms indicative imperative : MoodForms =
    { Indicative = indicative
      Imperative = imperative }

let emptyMoodForms () : MoodForms =
    { Indicative = emptyVoiceForms ()
      Imperative = emptyVoiceForms () }

let participleTenseForms present past perfect : ParticipleTenseForms =
    { Present = present
      Past = past
      Perfect = perfect }

let emptyParticipleTenseForms () : ParticipleTenseForms =
    { Present = None
      Past = None
      Perfect = None }

let participleVoiceForms active passive : ParticipleVoiceForms =
    { Active = active
      Passive = passive }

let emptyParticipleVoiceForms () : ParticipleVoiceForms =
    { Active = emptyParticipleTenseForms ()
      Passive = emptyParticipleTenseForms () }

let participleForms voice : ParticipleForms = { Voice = voice }

let infinitiveForms active passive : InfinitiveForms =
    { Active = active
      Passive = passive }

let verbForms moods participles aoristInfinitives : VerbForms =
    { Moods = moods
      Participles = participles
      AoristInfinitives = aoristInfinitives }

let emptyVerbForms () : VerbForms =
    { Moods = emptyMoodForms ()
      Participles = { Voice = emptyParticipleVoiceForms () }
      AoristInfinitives = { Active = None; Passive = None } }

let verb lemma glosses conjugations examples : VerbInfo =
    { Lemma = lemma
      Glosses = glosses
      Conjugations = conjugations
      Examples = examples }
