namespace GreekReader

open System
open System.IO
open GreekReader.Model
open GreekReader.Parsing
open GreekReader.Wiktionary

module API =

    let private dataPath (partOfSpeech: string) : string =
        let filename = $"greek-{partOfSpeech}.jsonl"
        let candidates =
            [ Path.Combine(AppContext.BaseDirectory, "Resources", filename)
              Path.Combine("Resources", filename) ]
        candidates |> List.tryFind File.Exists |> Option.defaultValue candidates.Head

    let internal partsOfSpeech =
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

    let internal analyseWiktionaryData (words: Word seq) : AnalysedWord seq =
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
            | _ -> ParticleParser.analyse w)

    let internal parseWiktionaryData (partsOfSpeech: string seq) : Word seq =
        partsOfSpeech
        |> Seq.map dataPath
        |> Seq.map Jsonl.parseWords
        |> Seq.concat

    let query (query: string) : AnalysedWord list =
        partsOfSpeech
        |> parseWiktionaryData
        |> analyseWiktionaryData
        |> Seq.choose (fun w ->
            match w with
            | Noun n -> Service.searchNoun query n
            | Verb v -> Service.searchVerb query v
            | Determiner d -> Service.searchDeterminer query d
            | Adjective adj -> Service.searchAdjective query adj
            | Adverb adv -> Service.searchAdverb query adv
            | Article art -> Service.searchArticle query art
            | Pronoun pron -> Service.searchPronoun query pron
            | Preposition prep -> Service.searchPreposition query prep
            | Conjunction conj -> Service.searchConjunction query conj
            | Interjection intj -> Service.searchInterjection query intj
            | Numeral num -> Service.searchNumeral query num
            | ProperNoun prop -> Service.searchProperNoun query prop
            | Particle part -> Service.searchParticle query part)
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
        