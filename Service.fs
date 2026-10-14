namespace GreekReader

open GreekReader.Model

module Service =
    
    let private contains (sub: string) (s: string) =
        s.Equals(sub, System.StringComparison.OrdinalIgnoreCase)

    let private matchNumbers (numbers: NumberForms) : string list =
        [ numbers.Singular; numbers.Plural ] |> List.choose id

    let private matchCases (cases: CaseForms) : string list =
        [ cases.Nominative; cases.Accusative; cases.Genitive; cases.Vocative ]
        |> List.collect matchNumbers

    let private matchGenders (genders: GenderForms) : string list =
        [ genders.Masculine; genders.Feminine; genders.Neuter ]
        |> List.collect matchCases

    let private matchDegrees (degrees: DegreeForms) : string list =
        [ degrees.Positive; degrees.Comparative; degrees.Superlative ]
        |> List.collect matchGenders

    let private matchAdverbDegrees (degrees: AdverbDegrees) : string list =
        [ degrees.Comparative; degrees.Superlative ] |> List.choose id

    let private matchNumeralCases (cases: NumeralCases) : string list =
        [ cases.Nominative; cases.Accusative; cases.Genitive; cases.Vocative ]
        |> List.choose id

    let private matchNumeralGenders (genders: NumeralGenders) : string list =
        [ genders.Masculine; genders.Feminine; genders.Neuter ]
        |> List.collect matchNumeralCases

    let private matchPersons (persons: PersonForms) : string list =
        [ persons.FirstPerson; persons.SecondPerson; persons.ThirdPerson ]
        |> List.collect matchNumbers

    let private matchTenses (tenses: TenseForms) : string list =
        [ tenses.Present
          tenses.Imperfect
          tenses.Past
          tenses.Future
          tenses.Dependent ]
        |> List.collect matchPersons

    let private matchAspects (aspects: AspectForms) : string list =
        [ aspects.Imperfective; aspects.Perfective ] |> List.collect matchTenses

    let private matchVoices (voices: VoiceForms) : string list =
        [ voices.Active; voices.Passive ] |> List.collect matchAspects

    let private matchMoods (moods: MoodForms) : string list =
        [ moods.Indicative; moods.Imperative ] |> List.collect matchVoices

    let private matchParticipleTenses (tenses: ParticipleTenseForms) : string list =
        [ tenses.Present; tenses.Past; tenses.Perfect ] |> List.choose id

    let private matchParticipleVoices (voices: ParticipleVoiceForms) : string list =
        [ voices.Active; voices.Passive ] |> List.collect matchParticipleTenses

    let private matchInfinitives (infinitives: InfinitiveForms) : string list =
        [ infinitives.Active; infinitives.Passive ] |> List.choose id

    let private matchConjugations (conjugations: VerbForms) : string list =
        matchMoods conjugations.Moods
        |> List.append (matchParticipleVoices conjugations.Participles.Voice)
        |> List.append (matchInfinitives conjugations.AoristInfinitives)

    let searchNoun (query: string) (word: NounInfo) : AnalysedWord option =
        match
            word.Declensions
            |> matchCases
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Noun word)
        | _ -> None

    let searchDeterminer (query: string) (word: DeterminerInfo) : AnalysedWord option =
        match
            word.Declensions
            |> matchGenders
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Determiner word)
        | _ -> None

    let searchVerb (query: string) (word: VerbInfo) : AnalysedWord option =
        match
            word.Conjugations
            |> matchConjugations
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Verb word)
        | _ -> None

    let searchAdjective (query: string) (word: AdjectiveInfo) : AnalysedWord option =
        match
            word.Degrees
            |> matchDegrees
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Adjective word)
        | _ -> None

    let searchAdverb (query: string) (word: AdverbInfo) : AnalysedWord option =
        match
            word.Degrees
            |> matchAdverbDegrees
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Adverb word)
        | _ -> None

    let searchArticle (query: string) (word: ArticleInfo) : AnalysedWord option =
        match
            word.Inflections
            |> matchGenders
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Article word)
        | _ -> None

    let searchPronoun (query: string) (word: PronounInfo) : AnalysedWord option =
        match
            match word.Declensions with
            | GenderDeclined g -> g |> matchGenders
            | NonGenderDeclined c -> c |> matchCases
            |> List.append [ word.Lemma ]
            |> List.exists (contains query)
        with
        | true -> Some(Pronoun word)
        | _ -> None

    let searchPreposition (query: string) (word: PrepositionInfo) : AnalysedWord option =
        match [ word.Lemma ] |> List.exists (contains query) with
        | true -> Some(Preposition word)
        | _ -> None

    let searchConjunction (query: string) (word: ConjunctionInfo) : AnalysedWord option =
        match [ word.Lemma ] |> List.exists (contains query) with
        | true -> Some(Conjunction word)
        | _ -> None

    let searchInterjection (query: string) (word: InterjectionInfo) : AnalysedWord option =
        match [ word.Lemma ] |> List.exists (contains query) with
        | true -> Some(Interjection word)
        | _ -> None

    let searchNumeral (query: string) (word: NumeralInfo) : AnalysedWord option =
        match
            [ word.Lemma ]
            |> List.append (
                match word.Declensions with
                | Some genders -> matchNumeralGenders genders
                | None -> []
            )
            |> List.exists (contains query)
        with
        | true -> Some(Numeral word)
        | _ -> None

    let searchProperNoun (query: string) (word: ProperNounInfo) : AnalysedWord option =
        match
            [ word.Lemma ]
            |> List.append (
                match word.Declensions with
                | Some cases -> matchCases cases
                | None -> []
            )
            |> List.exists (contains query)
        with
        | true -> Some(ProperNoun word)
        | _ -> None

    let searchParticle (query: string) (word: ParticleInfo) : AnalysedWord option =
        match [ word.Lemma ] |> List.exists (contains query) with
        | true -> Some(Particle word)
        | _ -> None
