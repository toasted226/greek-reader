namespace GreekReader.Interop

open System
open System.Collections.Generic


module Convert =

    let private mapGender (gender: GreekReader.Model.Gender) : Gender =
        match gender with
        | GreekReader.Model.Gender.Masculine -> Gender.Masculine
        | GreekReader.Model.Gender.Feminine -> Gender.Feminine
        | GreekReader.Model.Gender.Neuter -> Gender.Neuter

    let private mapGenderOption (gender: GreekReader.Model.Gender option) : Nullable<Gender> =
        match gender with
        | Some value -> Nullable<Gender>(mapGender value)
        | None -> Nullable<Gender>()

    let private mapExample (example: GreekReader.Model.Example) : Example =
        { Greek = example.Greek
          English = example.English }

    let private mapExamples (examples: GreekReader.Model.Example list) : IReadOnlyList<Example> =
        examples |> List.map mapExample |> List.toArray :> IReadOnlyList<Example>

    let private mapNumberForms (forms: GreekReader.Model.NumberForms) : NumberForms =
        { Singular = Option.toObj forms.Singular
          Plural = Option.toObj forms.Plural }

    let private mapCaseForms (forms: GreekReader.Model.CaseForms) : CaseForms =
        { Nominative = mapNumberForms forms.Nominative
          Accusative = mapNumberForms forms.Accusative
          Genitive = mapNumberForms forms.Genitive
          Vocative = mapNumberForms forms.Vocative }

    let private mapGenderForms (forms: GreekReader.Model.GenderForms) : GenderForms =
        { Masculine = mapCaseForms forms.Masculine
          Feminine = mapCaseForms forms.Feminine
          Neuter = mapCaseForms forms.Neuter }

    let private mapPersonForms (forms: GreekReader.Model.PersonForms) : PersonForms =
        { FirstPerson = mapNumberForms forms.FirstPerson
          SecondPerson = mapNumberForms forms.SecondPerson
          ThirdPerson = mapNumberForms forms.ThirdPerson }

    let private mapTenseForms (forms: GreekReader.Model.TenseForms) : TenseForms =
        { Present = mapPersonForms forms.Present
          Imperfect = mapPersonForms forms.Imperfect
          Past = mapPersonForms forms.Past
          Future = mapPersonForms forms.Future
          Dependent = mapPersonForms forms.Dependent }

    let private mapAspectForms (forms: GreekReader.Model.AspectForms) : AspectForms =
        { Imperfective = mapTenseForms forms.Imperfective
          Perfective = mapTenseForms forms.Perfective }

    let private mapVoiceForms (forms: GreekReader.Model.VoiceForms) : VoiceForms =
        { Active = mapAspectForms forms.Active
          Passive = mapAspectForms forms.Passive }

    let private mapMoodForms (forms: GreekReader.Model.MoodForms) : MoodForms =
        { Indicative = mapVoiceForms forms.Indicative
          Imperative = mapVoiceForms forms.Imperative }

    let private mapParticipleTenseForms (forms: GreekReader.Model.ParticipleTenseForms) : ParticipleTenseForms =
        { Present = Option.toObj forms.Present
          Past = Option.toObj forms.Past
          Perfect = Option.toObj forms.Perfect }

    let private mapParticipleVoiceForms (forms: GreekReader.Model.ParticipleVoiceForms) : ParticipleVoiceForms =
        { Active = mapParticipleTenseForms forms.Active
          Passive = mapParticipleTenseForms forms.Passive }

    let private mapParticipleForms (forms: GreekReader.Model.ParticipleForms) : ParticipleForms =
        { Voice = mapParticipleVoiceForms forms.Voice }

    let private mapInfinitiveForms (forms: GreekReader.Model.InfinitiveForms) : InfinitiveForms =
        { Active = Option.toObj forms.Active
          Passive = Option.toObj forms.Passive }

    let private mapDegreeForms (forms: GreekReader.Model.DegreeForms) : DegreeForms =
        { Positive = mapGenderForms forms.Positive
          Comparative = mapGenderForms forms.Comparative
          Superlative = mapGenderForms forms.Superlative }

    let private mapVerbForms (forms: GreekReader.Model.VerbForms) : VerbForms =
        { Moods = mapMoodForms forms.Moods
          Participles = mapParticipleForms forms.Participles
          AoristInfinitives = mapInfinitiveForms forms.AoristInfinitives }

    let private mapAdverbDegrees (forms: GreekReader.Model.AdverbDegrees) : AdverbDegrees =
        { Comparative = Option.toObj forms.Comparative
          Superlative = Option.toObj forms.Superlative }

    let private mapNumeralCases (forms: GreekReader.Model.NumeralCases) : NumeralCases =
        { Nominative = Option.toObj forms.Nominative
          Accusative = Option.toObj forms.Accusative
          Genitive = Option.toObj forms.Genitive
          Vocative = Option.toObj forms.Vocative }

    let private mapNumeralGenders (forms: GreekReader.Model.NumeralGenders) : NumeralGenders =
        { Masculine = mapNumeralCases forms.Masculine
          Feminine = mapNumeralCases forms.Feminine
          Neuter = mapNumeralCases forms.Neuter }

    let private setBase (lemma: string) (glosses: string list) (word: #AnalysedWord) =
        word.Lemma <- lemma
        word.Glosses <- (List.toArray glosses :> IReadOnlyList<string>)
        word

    let toInterop (word: GreekReader.Model.AnalysedWord) : AnalysedWord =
        match word with
        | GreekReader.Model.AnalysedWord.Noun n ->
            let w = setBase n.Lemma n.Glosses (NounWord())
            w.Gender <- mapGenderOption n.Gender
            w.Declensions <- mapCaseForms n.Declensions
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Determiner d ->
            let w = setBase d.Lemma d.Glosses (DeterminerWord())
            w.Declensions <- mapGenderForms d.Declensions
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Verb v ->
            let w = setBase v.Lemma v.Glosses (VerbWord())
            w.Conjugations <- mapVerbForms v.Conjugations
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Adjective adj ->
            let w = setBase adj.Lemma adj.Glosses (AdjectiveWord())
            w.Degrees <- mapDegreeForms adj.Degrees
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Adverb adv ->
            let w = setBase adv.Lemma adv.Glosses (AdverbWord())
            w.Degrees <- mapAdverbDegrees adv.Degrees
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Article art ->
            let w = setBase art.Lemma art.Glosses (ArticleWord())
            w.Kind <-
                match art.Type with
                | GreekReader.Model.ArticleType.Definite -> ArticleKind.Definite
                | GreekReader.Model.ArticleType.Indefinite -> ArticleKind.Indefinite
            w.Inflections <- mapGenderForms art.Inflections
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Pronoun p ->
            let w = setBase p.Lemma p.Glosses (PronounWord())
            match p.Declensions with
            | GreekReader.Model.PronounDeclensions.GenderDeclined g -> w.GenderDeclensions <- mapGenderForms g
            | GreekReader.Model.PronounDeclensions.NonGenderDeclined c -> w.CaseDeclensions <- mapCaseForms c
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Preposition p ->
            let w = setBase p.Lemma p.Glosses (PrepositionWord())
            w.Examples <- mapExamples p.Examples
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Conjunction c ->
            let w = setBase c.Lemma c.Glosses (ConjunctionWord())
            w.Examples <- mapExamples c.Examples
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Interjection i ->
            let w = setBase i.Lemma i.Glosses (InterjectionWord())
            w.Examples <- mapExamples i.Examples
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Numeral n ->
            let w = setBase n.Lemma n.Glosses (NumeralWord())
            w.Declensions <-
                match n.Declensions with
                | Some forms -> mapNumeralGenders forms
                | None -> null
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.ProperNoun p ->
            let w = setBase p.Lemma p.Glosses (ProperNounWord())
            w.Declensions <-
                match p.Declensions with
                | Some forms -> mapCaseForms forms
                | None -> null
            w :> AnalysedWord
        | GreekReader.Model.AnalysedWord.Particle p ->
            let w = setBase p.Lemma p.Glosses (ParticleWord())
            w.Examples <- mapExamples p.Examples
            w :> AnalysedWord
