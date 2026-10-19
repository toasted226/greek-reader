namespace GreekReader.Interop

module Convert =

    open GreekReader.Model

    let private bracketPattern = System.Text.RegularExpressions.Regex(@"\(([^)]*)\)")

    let private splitDelimited (s: string) : string list =
        s.Split([| " - " |], System.StringSplitOptions.None)
        |> Array.map (fun x -> x.Trim())
        |> Array.filter (fun x -> not (System.String.IsNullOrWhiteSpace x))
        |> Array.toList

    let private extractBracketed (s: string) : string list =
        bracketPattern.Matches(s)
        |> Seq.cast<System.Text.RegularExpressions.Match>
        |> Seq.map (fun m -> m.Groups.[1].Value.Trim())
        |> Seq.filter (fun x -> not (System.String.IsNullOrWhiteSpace x))
        |> Seq.toList

    let private removeBrackets (s: string) : string =
        bracketPattern.Replace(s, "").Trim()

    let private expandVariants (features: string list) (s: string) : FormEntry list =
        let main = removeBrackets s
        let bracketed = extractBracketed s

        [ if not (System.String.IsNullOrWhiteSpace main) then
            { Form = main; Features = List.toArray features }
          for b in bracketed do
            { Form = b; Features = List.toArray features } ]

    let private form (features: string list) (value: Variants) : FormEntry list =
        value
        |> List.collect (fun v ->
            if System.String.IsNullOrWhiteSpace v then
                []
            else
                v
                |> splitDelimited
                |> List.collect (expandVariants features))
        |> List.distinctBy (fun f -> f.Form, f.Features)

    let private numberForms (features: string list) (nf: NumberForms) : FormEntry list =
        [ yield! form (features @ [ "singular" ]) nf.Singular
          yield! form (features @ [ "plural" ]) nf.Plural ]

    let private caseForms (features: string list) (cf: CaseForms) : FormEntry list =
        [ yield! numberForms (features @ [ "nominative" ]) cf.Nominative
          yield! numberForms (features @ [ "accusative" ]) cf.Accusative
          yield! numberForms (features @ [ "genitive" ]) cf.Genitive
          yield! numberForms (features @ [ "vocative" ]) cf.Vocative ]

    let private genderForms (features: string list) (gf: GenderForms) : FormEntry list =
        [ yield! caseForms (features @ [ "masculine" ]) gf.Masculine
          yield! caseForms (features @ [ "feminine" ]) gf.Feminine
          yield! caseForms (features @ [ "neuter" ]) gf.Neuter ]

    let private degreeForms (df: DegreeForms) : FormEntry list =
        [ yield! genderForms [ "positive" ] df.Positive
          yield! genderForms [ "comparative" ] df.Comparative
          yield! genderForms [ "superlative" ] df.Superlative ]

    let private genderFeatures (gender: Gender option) : string list =
        match gender with
        | Some Masculine -> [ "masculine" ]
        | Some Feminine -> [ "feminine" ]
        | Some Neuter -> [ "neuter" ]
        | None -> []

    let private examples (xs: Example list) : GreekReader.Interop.Example[] =
        xs
        |> List.map (fun e -> ({ Greek = e.Greek; English = e.English }: GreekReader.Interop.Example))
        |> List.toArray

    let private noun (n: NounInfo) : LemmaEntry =
        { Lemma = n.Lemma
          Pos = PartOfSpeech.Noun
          Glosses = List.toArray n.Glosses
          Examples = examples n.Examples
          Forms = caseForms (genderFeatures n.Gender) n.Declensions |> List.toArray }

    let private article (a: ArticleInfo) : LemmaEntry =
        let kind =
            match a.Type with
            | Definite -> "definite"
            | Indefinite -> "indefinite"

        { Lemma = a.Lemma
          Pos = PartOfSpeech.Article
          Glosses = List.toArray a.Glosses
          Examples = examples a.Examples
          Forms = genderForms [ kind ] a.Inflections |> List.toArray }

    let private adjective (a: AdjectiveInfo) : LemmaEntry =
        { Lemma = a.Lemma
          Pos = PartOfSpeech.Adjective
          Glosses = List.toArray a.Glosses
          Examples = examples a.Examples
          Forms = degreeForms a.Degrees |> List.toArray }

    let private determiner (d: DeterminerInfo) : LemmaEntry =
        { Lemma = d.Lemma
          Pos = PartOfSpeech.Determiner
          Glosses = List.toArray d.Glosses
          Examples = examples d.Examples
          Forms = genderForms [] d.Declensions |> List.toArray }

    let private pronoun (p: PronounInfo) : LemmaEntry =
        let forms =
            match p.Declensions with
            | GenderDeclined gf -> genderForms [] gf
            | NonGenderDeclined cf -> caseForms [] cf

        { Lemma = p.Lemma
          Pos = PartOfSpeech.Pronoun
          Glosses = List.toArray p.Glosses
          Examples = examples p.Examples
          Forms = forms |> List.toArray }

    let private preposition (p: PrepositionInfo) : LemmaEntry =
        { Lemma = p.Lemma
          Pos = PartOfSpeech.Preposition
          Glosses = List.toArray p.Glosses
          Examples = examples p.Examples
          Forms = [||] }

    let private conjunction (c: ConjunctionInfo) : LemmaEntry =
        { Lemma = c.Lemma
          Pos = PartOfSpeech.Conjunction
          Glosses = List.toArray c.Glosses
          Examples = examples c.Examples
          Forms = [||] }

    let private adverbDegrees (d: AdverbDegrees) : FormEntry list =
        [ yield! form [ "comparative" ] d.Comparative
          yield! form [ "superlative" ] d.Superlative ]

    let private adverb (a: AdverbInfo) : LemmaEntry =
        { Lemma = a.Lemma
          Pos = PartOfSpeech.Adverb
          Glosses = List.toArray a.Glosses
          Examples = examples a.Examples
          Forms = adverbDegrees a.Degrees |> List.toArray }

    let private interjection (i: InterjectionInfo) : LemmaEntry =
        { Lemma = i.Lemma
          Pos = PartOfSpeech.Interjection
          Glosses = List.toArray i.Glosses
          Examples = examples i.Examples
          Forms = [||] }

    let private numeralCases (features: string list) (nc: NumeralCases) : FormEntry list =
        [ yield! form (features @ [ "nominative" ]) nc.Nominative
          yield! form (features @ [ "accusative" ]) nc.Accusative
          yield! form (features @ [ "genitive" ]) nc.Genitive
          yield! form (features @ [ "vocative" ]) nc.Vocative ]

    let private numeralGenders (ng: NumeralGenders) : FormEntry list =
        [ yield! numeralCases [ "masculine" ] ng.Masculine
          yield! numeralCases [ "feminine" ] ng.Feminine
          yield! numeralCases [ "neuter" ] ng.Neuter ]

    let private numeral (n: NumeralInfo) : LemmaEntry =
        let forms = n.Declensions |> Option.map numeralGenders |> Option.defaultValue []

        { Lemma = n.Lemma
          Pos = PartOfSpeech.Numeral
          Glosses = List.toArray n.Glosses
          Examples = examples n.Examples
          Forms = forms |> List.toArray }

    let private properNoun (p: ProperNounInfo) : LemmaEntry =
        let forms = p.Declensions |> Option.map (caseForms []) |> Option.defaultValue []

        { Lemma = p.Lemma
          Pos = PartOfSpeech.ProperNoun
          Glosses = List.toArray p.Glosses
          Examples = examples p.Examples
          Forms = forms |> List.toArray }

    let private particle (p: ParticleInfo) : LemmaEntry =
        { Lemma = p.Lemma
          Pos = PartOfSpeech.Particle
          Glosses = List.toArray p.Glosses
          Examples = examples p.Examples
          Forms = [||] }

    let private personForms (features: string list) (pf: PersonForms) : FormEntry list =
        [ yield! numberForms (features @ [ "first-person" ]) pf.FirstPerson
          yield! numberForms (features @ [ "second-person" ]) pf.SecondPerson
          yield! numberForms (features @ [ "third-person" ]) pf.ThirdPerson ]

    let private tenseForms (features: string list) (tf: TenseForms) : FormEntry list =
        [ yield! personForms (features @ [ "present" ]) tf.Present
          yield! personForms (features @ [ "imperfect" ]) tf.Imperfect
          yield! personForms (features @ [ "past" ]) tf.Past
          yield! personForms (features @ [ "future" ]) tf.Future
          yield! personForms (features @ [ "dependent" ]) tf.Dependent ]

    let private aspectForms (features: string list) (af: AspectForms) : FormEntry list =
        [ yield! tenseForms (features @ [ "imperfective" ]) af.Imperfective
          yield! tenseForms (features @ [ "perfective" ]) af.Perfective ]

    let private voiceForms (features: string list) (vf: VoiceForms) : FormEntry list =
        [ yield! aspectForms (features @ [ "active" ]) vf.Active
          yield! aspectForms (features @ [ "passive" ]) vf.Passive ]

    let private moodForms (mf: MoodForms) : FormEntry list =
        [ yield! voiceForms [ "indicative" ] mf.Indicative
          yield! voiceForms [ "imperative" ] mf.Imperative ]

    let private participleTenseForms (voice: string) (pt: ParticipleTenseForms) : FormEntry list =
        [ yield! form [ "participle"; voice; "present" ] pt.Present
          yield! form [ "participle"; voice; "past" ] pt.Past
          yield! form [ "participle"; voice; "perfect" ] pt.Perfect ]

    let private participleForms (pf: ParticipleForms) : FormEntry list =
        [ yield! participleTenseForms "active" pf.Voice.Active
          yield! participleTenseForms "passive" pf.Voice.Passive ]

    let private infinitiveForms (inf: InfinitiveForms) : FormEntry list =
        [ yield! form [ "infinitive"; "active"; "aorist" ] inf.Active
          yield! form [ "infinitive"; "passive"; "aorist" ] inf.Passive ]

    let private verb (v: VerbInfo) : LemmaEntry =
        let forms =
            [ yield! moodForms v.Conjugations.Moods
              yield! participleForms v.Conjugations.Participles
              yield! infinitiveForms v.Conjugations.AoristInfinitives ]

        { Lemma = v.Lemma
          Pos = PartOfSpeech.Verb
          Glosses = List.toArray v.Glosses
          Examples = examples v.Examples
          Forms = forms |> List.toArray }

    let toInterop (word: AnalysedWord) : LemmaEntry =
        let entry =
            match word with
            | Noun n -> noun n
            | Article a -> article a
            | Adjective a -> adjective a
            | Determiner d -> determiner d
            | Pronoun p -> pronoun p
            | Preposition p -> preposition p
            | Conjunction c -> conjunction c
            | Adverb a -> adverb a
            | Interjection i -> interjection i
            | Numeral n -> numeral n
            | ProperNoun p -> properNoun p
            | Particle p -> particle p
            | Verb v -> verb v

        if Array.isEmpty entry.Forms then
            { entry with Forms = [| { Form = entry.Lemma; Features = [||] } |] }
        else
            entry