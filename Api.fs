namespace GreekReader

open GreekReader.Model
open GreekReader.Parsing
open GreekReader.Wiktionary

module API =

    let private dataPath (partOfSpeech: string) : string = $"Resources/greek-{partOfSpeech}.jsonl"

    let private partsOfSpeech =
        [ "verbs"
          "adjectives"
          "articles"
          "nouns"
          "determiners"
          "adverbs"
          "pronouns"
          "prepositions"
          "conjunctions"
          "interjections"
          "numerals"
          "proper-nouns"
          "particles" ]

    let private analyseWiktionaryData (words: Word seq) : AnalysedWord seq =
        words
        |> Seq.map (fun w ->
            match w.Pos with
            | "verb" -> VerbParser.analyse w
            | "adj" -> AdjectiveParser.analyse w
            | "article" -> ArticleParser.analyse w
            | "noun" -> NounParser.analyse w
            | "det" -> DeterminerParser.analyse w
            | "adv" -> AdverbParser.analyse w
            | "pron" -> PronounParser.analyse w
            | "prep" -> PrepositionParser.analyse w
            | "conj" -> ConjunctionParser.analyse w
            | "intj" -> InterjectionParser.analyse w
            | "num" -> NumeralParser.analyse w
            | "name" -> ProperNounParser.analyse w
            | "particle" -> ParticleParser.analyse w
            | _ -> InterjectionParser.analyse w)

    let private parseWiktionaryData (partsOfSpeech: string seq) : Word seq =
        partsOfSpeech |> Seq.map dataPath |> Seq.map Jsonl.parseWords |> Seq.concat

    let contains (sub: string) (s: string) =
        s.Equals(sub, System.StringComparison.OrdinalIgnoreCase)

    let matchNumbers (numbers: NumberForms) : string list =
        [ numbers.Singular; numbers.Plural ] |> List.choose id

    let matchCases (cases: CaseForms) : string list =
        [ cases.Nominative; cases.Accusative; cases.Genitive; cases.Vocative ]
        |> List.collect matchNumbers

    let matchGenders (genders: GenderForms) : string list =
        [ genders.Masculine; genders.Feminine; genders.Neuter ]
        |> List.collect matchCases

    let matchDegrees (degrees: DegreeForms) : string list =
        [ degrees.Positive; degrees.Comparative; degrees.Superlative ]
        |> List.collect matchGenders

    let matchAdverbDegrees (degrees: AdverbDegrees) : string list =
        [ degrees.Comparative; degrees.Superlative ] |> List.choose id

    let matchNumeralCases (cases: NumeralCases) : string list =
        [ cases.Nominative; cases.Accusative; cases.Genitive; cases.Vocative ]
        |> List.choose id

    let matchNumeralGenders (genders: NumeralGenders) : string list =
        [ genders.Masculine; genders.Feminine; genders.Neuter ]
        |> List.collect matchNumeralCases

    let matchPersons (persons: PersonForms) : string list =
        [ persons.FirstPerson; persons.SecondPerson; persons.ThirdPerson ]
        |> List.collect matchNumbers

    let matchTenses (tenses: TenseForms) : string list =
        [ tenses.Present
          tenses.Imperfect
          tenses.Past
          tenses.Future
          tenses.Dependent ]
        |> List.collect matchPersons

    let matchAspects (aspects: AspectForms) : string list =
        [ aspects.Imperfective; aspects.Perfective ] |> List.collect matchTenses

    let matchVoices (voices: VoiceForms) : string list =
        [ voices.Active; voices.Passive ] |> List.collect matchAspects

    let matchMoods (moods: MoodForms) : string list =
        [ moods.Indicative; moods.Imperative ] |> List.collect matchVoices

    let matchParticipleTenses (tenses: ParticipleTenseForms) : string list =
        [ tenses.Present; tenses.Past; tenses.Perfect ] |> List.choose id

    let matchParticipleVoices (voices: ParticipleVoiceForms) : string list =
        [ voices.Active; voices.Passive ] |> List.collect matchParticipleTenses

    let matchInfinitives (infinitives: InfinitiveForms) : string list =
        [ infinitives.Active; infinitives.Passive ] |> List.choose id

    let matchConjugations (conjugations: VerbForms) : string list =
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
        let matches =
            match word.Declensions with
            | GenderDeclined g -> g |> matchGenders
            | NonGenderDeclined c -> c |> matchCases

        match matches |> List.append [ word.Lemma ] |> List.exists (contains query) with
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
    
    let query (query: string) : AnalysedWord list =
        partsOfSpeech
        |> parseWiktionaryData
        |> analyseWiktionaryData
        |> Seq.choose (fun w ->
            match w with
            | Noun n -> searchNoun query n
            | Verb v -> searchVerb query v
            | Determiner d -> searchDeterminer query d
            | Adjective adj -> searchAdjective query adj
            | Adverb adv -> searchAdverb query adv
            | Article art -> searchArticle query art
            | Pronoun pron -> searchPronoun query pron
            | Preposition prep -> searchPreposition query prep
            | Conjunction conj -> searchConjunction query conj
            | Interjection intj -> searchInterjection query intj
            | Numeral num -> searchNumeral query num
            | ProperNoun prop -> searchProperNoun query prop
            | Particle part -> searchParticle query part
            )
        |> Seq.toList

    let queryText (text: string) : Map<string, AnalysedWord list> =
        text.Split([| ' '; '.'; ','; ';'; '?'; '!'; ';'; '\n'; '"'; ''' |])
        |> Array.toSeq
        |> Seq.map _.ToLower()
        |> Seq.map _.Trim()
        |> Seq.choose (fun q ->
            match query q with
            | [] -> None
            | ws -> Some(q, ws))
        |> Map.ofSeq
