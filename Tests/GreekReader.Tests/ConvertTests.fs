module GreekReader.Tests.ConvertTests

open Xunit
open GreekReader.Model
open GreekReader.Interop
open GreekReader.Tests.Fixtures

let private form (entry: LemmaEntry) (text: string) =
    entry.Forms |> Array.find (fun f -> f.Form = text)

let private features (entry: LemmaEntry) (text: string) = (form entry text).Features
let private forms (entry: LemmaEntry) = entry.Forms |> Array.map _.Form

let private fullNoun gender =
    Convert.toInterop (
        Noun(
            noun
                "λόγος"
                [ "word" ]
                gender
                (cases
                    (nf (Some "λόγος") (Some "λόγοι"))
                    (nf (Some "λόγον") (Some "λόγους"))
                    (nf (Some "λόγου") (Some "λόγων"))
                    (nf (Some "λόγε") (Some "λόγοι")))
                [ ex "ὁ λόγος" "the word" ]
        )
    )

[<Fact>]
let ``Noun maps scalar fields`` () =
    let entry =
        Convert.toInterop (
            Noun(
                noun
                    "λόγος"
                    [ "word"; "reason" ]
                    (Some Masculine)
                    (cases
                        (nf (Some "λόγος") (Some "λόγοι"))
                        (nf (Some "λόγον") None)
                        (nf None (Some "λόγων"))
                        (nf (Some "λόγε") None))
                    [ ex "ὁ λόγος" "the word" ]
            )
        )

    Assert.Equal(PartOfSpeech.Noun, entry.Pos)
    Assert.Equal("λόγος", entry.Lemma)
    Assert.Equal<string[]>([| "word"; "reason" |], entry.Glosses)
    Assert.Equal("ὁ λόγος", (Assert.Single entry.Examples).Greek)
    Assert.Equal("the word", entry.Examples[0].English)

[<Fact>]
let ``Noun emits features in gender case number order`` () =
    let entry = fullNoun (Some Masculine)
    Assert.Equal<string[]>([| "masculine"; "nominative"; "singular" |], features entry "λόγος")

[<Fact>]
let ``Noun omits gender feature when absent`` () =
    let entry = fullNoun None
    Assert.Equal<string[]>([| "nominative"; "singular" |], features entry "λόγος")

[<Fact>]
let ``Noun skips absent and blank forms`` () =
    let entry =
        Convert.toInterop (
            Noun(
                noun
                    "λόγος"
                    [ "word" ]
                    None
                    (cases
                        (nf (Some "λόγος") (Some "  "))
                        (nf None None)
                        (nf (Some "λόγου") None)
                        (nf None None))
                    []
            )
        )

    Assert.Equal(2, entry.Forms.Length)
    Assert.DoesNotContain(entry.Forms, fun f -> System.String.IsNullOrWhiteSpace f.Form)

[<Fact>]
let ``Noun emits forms in case then number order`` () =
    let entry =
        Convert.toInterop (
            Noun(
                noun
                    "λόγος"
                    []
                    None
                    (cases
                        (nf (Some "λόγος") (Some "λόγοι"))
                        (nf (Some "λόγον") (Some "λόγους"))
                        (nf (Some "λόγου") (Some "λόγων"))
                        (nf (Some "λόγε") (Some "λόγοι")))
                    []
            )
        )

    Assert.Equal<string[]>(
        [| "λόγος"; "λόγοι"; "λόγον"; "λόγους"; "λόγου"; "λόγων"; "λόγε"; "λόγοι" |],
        forms entry
    )

[<Fact>]
let ``Noun has empty forms when no declensions`` () =
    let entry = Convert.toInterop (Noun(noun "λόγος" [] None (noCases ()) []))
    Assert.Empty(entry.Forms)

let private fullArticle kind =
    Convert.toInterop (
        Article(
            article
                "ὁ"
                [ "the" ]
                kind
                (genderForms
                    (cases
                        (nf (Some "ὁ") (Some "οἱ"))
                        (nf (Some "τόν") (Some "τούς"))
                        (nf (Some "τοῦ") (Some "τῶν"))
                        (nf (Some "ὦ") (Some "ὦ")))
                    (cases
                        (nf (Some "ἡ") (Some "αἱ"))
                        (nf (Some "τήν") (Some "τάς"))
                        (nf (Some "τῆς") (Some "τῶν"))
                        (nf (Some "ὦ") (Some "ὦ")))
                    (cases
                        (nf (Some "τό") (Some "τά"))
                        (nf (Some "τό") (Some "τά"))
                        (nf (Some "τοῦ") (Some "τῶν"))
                        (nf (Some "τό") (Some "τά"))))
                [ ex "ὁ λόγος" "the word" ]
        )
    )

[<Fact>]
let ``Article maps scalar fields`` () =
    let entry = fullArticle Definite

    Assert.Equal(PartOfSpeech.Article, entry.Pos)
    Assert.Equal("ὁ", entry.Lemma)
    Assert.Equal<string[]>([| "the" |], entry.Glosses)
    Assert.Equal("ὁ λόγος", (Assert.Single entry.Examples).Greek)
    Assert.Equal("the word", entry.Examples[0].English)

[<Fact>]
let ``Article emits kind gender case number order`` () =
    let entry = fullArticle Definite

    Assert.Equal<string[]>(
        [| "definite"; "masculine"; "nominative"; "singular" |],
        features entry "ὁ"
    )

[<Fact>]
let ``Article reports indefinite kind`` () =
    let entry = fullArticle Indefinite

    Assert.Equal<string[]>(
        [| "indefinite"; "masculine"; "nominative"; "singular" |],
        features entry "ὁ"
    )

[<Fact>]
let ``Article emits forms in gender case number order`` () =
    let entry = fullArticle Definite

    Assert.Equal<string[]>(
        [| "ὁ"; "οἱ"; "τόν"; "τούς"; "τοῦ"; "τῶν"; "ὦ"; "ὦ"
           "ἡ"; "αἱ"; "τήν"; "τάς"; "τῆς"; "τῶν"; "ὦ"; "ὦ"
           "τό"; "τά"; "τό"; "τά"; "τοῦ"; "τῶν"; "τό"; "τά" |],
        forms entry
    )

[<Fact>]
let ``Article has empty forms when no inflections`` () =
    let entry = Convert.toInterop (Article(article "ὁ" [] Definite (noGenderForms ()) []))
    Assert.Empty(entry.Forms)

let private masculineNomSingular text : GenderForms =
    genderForms
        (cases (nf (Some text) None) (nf None None) (nf None None) (nf None None))
        (noCases ())
        (noCases ())

let private fullAdjective =
    Convert.toInterop (
        Adjective(
            adjective
                "ἀγαθός"
                [ "good" ]
                (degreeForms
                    (masculineNomSingular "ἀγαθός")
                    (masculineNomSingular "ἀμείνων")
                    (masculineNomSingular "ἄριστος"))
                [ ex "ἀγαθὸς ἀνήρ" "a good man" ]
        )
    )

[<Fact>]
let ``Adjective maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Adjective, fullAdjective.Pos)
    Assert.Equal("ἀγαθός", fullAdjective.Lemma)
    Assert.Equal<string[]>([| "good" |], fullAdjective.Glosses)
    Assert.Equal("ἀγαθὸς ἀνήρ", (Assert.Single fullAdjective.Examples).Greek)
    Assert.Equal("a good man", fullAdjective.Examples[0].English)

[<Fact>]
let ``Adjective emits degree gender case number order`` () =
    Assert.Equal<string[]>(
        [| "positive"; "masculine"; "nominative"; "singular" |],
        features fullAdjective "ἀγαθός"
    )

[<Fact>]
let ``Adjective reports comparative and superlative degrees`` () =
    Assert.Equal<string[]>(
        [| "comparative"; "masculine"; "nominative"; "singular" |],
        features fullAdjective "ἀμείνων"
    )

    Assert.Equal<string[]>(
        [| "superlative"; "masculine"; "nominative"; "singular" |],
        features fullAdjective "ἄριστος"
    )

[<Fact>]
let ``Adjective emits forms in degree order`` () =
    Assert.Equal<string[]>([| "ἀγαθός"; "ἀμείνων"; "ἄριστος" |], forms fullAdjective)

[<Fact>]
let ``Adjective has empty forms when no degrees`` () =
    let entry =
        Convert.toInterop (Adjective(adjective "ἀγαθός" [] (noDegreeForms ()) []))

    Assert.Empty(entry.Forms)

let private fullDeterminer =
    Convert.toInterop (
        Determiner(
            determiner
                "οὗτος"
                [ "this" ]
                (genderForms
                    (cases
                        (nf (Some "οὗτος") (Some "οὗτοι"))
                        (nf (Some "τοῦτον") (Some "τούτους"))
                        (nf (Some "τούτου") (Some "τούτων"))
                        (nf (Some "οὗτος") (Some "οὗτοι")))
                    (cases
                        (nf (Some "αὕτη") (Some "αὗται"))
                        (nf (Some "ταύτην") (Some "ταύτας"))
                        (nf (Some "ταύτης") (Some "τούτων"))
                        (nf (Some "αὕτη") (Some "αὗται")))
                    (cases
                        (nf (Some "τοῦτο") (Some "ταῦτα"))
                        (nf (Some "τοῦτο") (Some "ταῦτα"))
                        (nf (Some "τούτου") (Some "τούτων"))
                        (nf (Some "τοῦτο") (Some "ταῦτα"))))
                [ ex "οὗτος ὁ ἀνήρ" "this man" ]
        )
    )

[<Fact>]
let ``Determiner maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Determiner, fullDeterminer.Pos)
    Assert.Equal("οὗτος", fullDeterminer.Lemma)
    Assert.Equal<string[]>([| "this" |], fullDeterminer.Glosses)
    Assert.Equal("οὗτος ὁ ἀνήρ", (Assert.Single fullDeterminer.Examples).Greek)
    Assert.Equal("this man", fullDeterminer.Examples[0].English)

[<Fact>]
let ``Determiner emits features in gender case number order`` () =
    Assert.Equal<string[]>(
        [| "masculine"; "nominative"; "singular" |],
        features fullDeterminer "οὗτος"
    )

[<Fact>]
let ``Determiner emits forms in gender case number order`` () =
    Assert.Equal<string[]>(
        [| "οὗτος"; "οὗτοι"; "τοῦτον"; "τούτους"; "τούτου"; "τούτων"; "οὗτος"; "οὗτοι"
           "αὕτη"; "αὗται"; "ταύτην"; "ταύτας"; "ταύτης"; "τούτων"; "αὕτη"; "αὗται"
           "τοῦτο"; "ταῦτα"; "τοῦτο"; "ταῦτα"; "τούτου"; "τούτων"; "τοῦτο"; "ταῦτα" |],
        forms fullDeterminer
    )

[<Fact>]
let ``Determiner has empty forms when no declensions`` () =
    let entry =
        Convert.toInterop (Determiner(determiner "οὗτος" [] (noGenderForms ()) []))

    Assert.Empty(entry.Forms)

let private fullPronounGender =
    Convert.toInterop (
        Pronoun(
            pronoun
                "αὐτός"
                [ "he"; "self" ]
                (GenderDeclined (masculineNomSingular "αὐτός"))
                [ ex "αὐτὸς λέγει" "he says" ]
        )
    )

let private fullPronounCase =
    Convert.toInterop (
        Pronoun(
            pronoun
                "ἐγώ"
                [ "I" ]
                (NonGenderDeclined (
                    cases
                        (nf (Some "ἐγώ") (Some "ἡμεῖς"))
                        (nf (Some "ἐμέ") (Some "ἡμᾶς"))
                        (nf (Some "ἐμοῦ") (Some "ἡμῶν"))
                        (nf (Some "ἐγώ") (Some "ἡμεῖς"))
                ))
                [ ex "ἐγὼ λέγω" "I say" ]
        )
    )

[<Fact>]
let ``Pronoun maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Pronoun, fullPronounGender.Pos)
    Assert.Equal("αὐτός", fullPronounGender.Lemma)
    Assert.Equal<string[]>([| "he"; "self" |], fullPronounGender.Glosses)
    Assert.Equal("αὐτὸς λέγει", (Assert.Single fullPronounGender.Examples).Greek)
    Assert.Equal("he says", fullPronounGender.Examples[0].English)

[<Fact>]
let ``Pronoun gender-declined emits gender case number order`` () =
    Assert.Equal<string[]>(
        [| "masculine"; "nominative"; "singular" |],
        features fullPronounGender "αὐτός"
    )

[<Fact>]
let ``Pronoun non-gender-declined emits case number order`` () =
    Assert.Equal<string[]>([| "nominative"; "singular" |], features fullPronounCase "ἐγώ")

[<Fact>]
let ``Pronoun non-gender-declined emits forms in case number order`` () =
    Assert.Equal<string[]>(
        [| "ἐγώ"; "ἡμεῖς"; "ἐμέ"; "ἡμᾶς"; "ἐμοῦ"; "ἡμῶν"; "ἐγώ"; "ἡμεῖς" |],
        forms fullPronounCase
    )

[<Fact>]
let ``Pronoun has empty forms when no declensions`` () =
    let genderDeclined =
        Convert.toInterop (Pronoun(pronoun "αὐτός" [] (GenderDeclined (noGenderForms ())) []))

    let caseDeclined =
        Convert.toInterop (Pronoun(pronoun "ἐγώ" [] (NonGenderDeclined (noCases ())) []))

    Assert.Empty(genderDeclined.Forms)
    Assert.Empty(caseDeclined.Forms)

[<Fact>]
let ``Preposition maps scalar fields`` () =
    let entry =
        Convert.toInterop (Preposition(preposition "ἐν" [ "in" ] [ ex "ἐν ἀρχῇ" "in the beginning" ]))

    Assert.Equal(PartOfSpeech.Preposition, entry.Pos)
    Assert.Equal("ἐν", entry.Lemma)
    Assert.Equal<string[]>([| "in" |], entry.Glosses)
    Assert.Equal("ἐν ἀρχῇ", (Assert.Single entry.Examples).Greek)
    Assert.Equal("in the beginning", entry.Examples[0].English)

[<Fact>]
let ``Preposition has no forms`` () =
    let entry = Convert.toInterop (Preposition(preposition "ἐν" [] []))
    Assert.Empty(entry.Forms)

[<Fact>]
let ``Conjunction maps scalar fields`` () =
    let entry =
        Convert.toInterop (Conjunction(conjunction "καί" [ "and"; "even" ] [ ex "καὶ σύ" "and you" ]))

    Assert.Equal(PartOfSpeech.Conjunction, entry.Pos)
    Assert.Equal("καί", entry.Lemma)
    Assert.Equal<string[]>([| "and"; "even" |], entry.Glosses)
    Assert.Equal("καὶ σύ", (Assert.Single entry.Examples).Greek)
    Assert.Equal("and you", entry.Examples[0].English)

[<Fact>]
let ``Conjunction has no forms`` () =
    let entry = Convert.toInterop (Conjunction(conjunction "καί" [] []))
    Assert.Empty(entry.Forms)

let private fullAdverb =
    Convert.toInterop (
        Adverb(
            adverb
                "εὖ"
                [ "well" ]
                (adverbDegrees (Some "ἄμεινον") (Some "ἄριστα"))
                [ ex "εὖ πράττει" "he does well" ]
        )
    )

[<Fact>]
let ``Adverb maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Adverb, fullAdverb.Pos)
    Assert.Equal("εὖ", fullAdverb.Lemma)
    Assert.Equal<string[]>([| "well" |], fullAdverb.Glosses)
    Assert.Equal("εὖ πράττει", (Assert.Single fullAdverb.Examples).Greek)
    Assert.Equal("he does well", fullAdverb.Examples[0].English)

[<Fact>]
let ``Adverb emits degree features`` () =
    Assert.Equal<string[]>([| "comparative" |], features fullAdverb "ἄμεινον")
    Assert.Equal<string[]>([| "superlative" |], features fullAdverb "ἄριστα")

[<Fact>]
let ``Adverb emits forms in degree order`` () =
    Assert.Equal<string[]>([| "ἄμεινον"; "ἄριστα" |], forms fullAdverb)

[<Fact>]
let ``Adverb has empty forms when no degrees`` () =
    let entry =
        Convert.toInterop (Adverb(adverb "εὖ" [] (adverbDegrees None None) []))

    Assert.Empty(entry.Forms)

[<Fact>]
let ``Interjection maps scalar fields`` () =
    let entry =
        Convert.toInterop (Interjection(interjection "οἴ" [ "woe" ] [ ex "οἴ μοι" "woe is me" ]))

    Assert.Equal(PartOfSpeech.Interjection, entry.Pos)
    Assert.Equal("οἴ", entry.Lemma)
    Assert.Equal<string[]>([| "woe" |], entry.Glosses)
    Assert.Equal("οἴ μοι", (Assert.Single entry.Examples).Greek)
    Assert.Equal("woe is me", entry.Examples[0].English)

[<Fact>]
let ``Interjection has no forms`` () =
    let entry = Convert.toInterop (Interjection(interjection "οἴ" [] []))
    Assert.Empty(entry.Forms)

let private fullNumeral =
    Convert.toInterop (
        Numeral(
            numeral
                "τρεῖς"
                [ "three" ]
                (Some(
                    numeralGenders
                        (numeralCases (Some "τρεῖς") (Some "τρεῖς") (Some "τριῶν") (Some "τρεῖς"))
                        (numeralCases (Some "τρεῖς") (Some "τρεῖς") (Some "τριῶν") (Some "τρεῖς"))
                        (numeralCases (Some "τρία") (Some "τρία") (Some "τριῶν") (Some "τρία"))
                ))
                [ ex "τρεῖς ἄνδρες" "three men" ]
        )
    )

[<Fact>]
let ``Numeral maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Numeral, fullNumeral.Pos)
    Assert.Equal("τρεῖς", fullNumeral.Lemma)
    Assert.Equal<string[]>([| "three" |], fullNumeral.Glosses)
    Assert.Equal("τρεῖς ἄνδρες", (Assert.Single fullNumeral.Examples).Greek)
    Assert.Equal("three men", fullNumeral.Examples[0].English)

[<Fact>]
let ``Numeral emits gender case order`` () =
    Assert.Equal<string[]>([| "masculine"; "nominative" |], features fullNumeral "τρεῖς")

[<Fact>]
let ``Numeral emits forms in gender then case order`` () =
    Assert.Equal<string[]>(
        [| "τρεῖς"; "τρεῖς"; "τριῶν"; "τρεῖς"
           "τρεῖς"; "τρεῖς"; "τριῶν"; "τρεῖς"
           "τρία"; "τρία"; "τριῶν"; "τρία" |],
        forms fullNumeral
    )

[<Fact>]
let ``Numeral has empty forms when no declensions`` () =
    let noDeclensions = Convert.toInterop (Numeral(numeral "τρεῖς" [] None []))
    let emptyDeclensions = Convert.toInterop (Numeral(numeral "τρεῖς" [] (Some(noNumeralGenders ())) []))

    Assert.Empty(noDeclensions.Forms)
    Assert.Empty(emptyDeclensions.Forms)

let private fullProperNoun =
    Convert.toInterop (
        ProperNoun(
            properNoun
                "Σωκράτης"
                [ "Socrates" ]
                (Some(
                    cases
                        (nf (Some "Σωκράτης") None)
                        (nf (Some "Σωκράτη") None)
                        (nf (Some "Σωκράτους") None)
                        (nf (Some "Σώκρατες") None)
                ))
                [ ex "Σωκράτης φιλοσοφεῖ" "Socrates philosophizes" ]
        )
    )

[<Fact>]
let ``ProperNoun maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.ProperNoun, fullProperNoun.Pos)
    Assert.Equal("Σωκράτης", fullProperNoun.Lemma)
    Assert.Equal<string[]>([| "Socrates" |], fullProperNoun.Glosses)
    Assert.Equal("Σωκράτης φιλοσοφεῖ", (Assert.Single fullProperNoun.Examples).Greek)
    Assert.Equal("Socrates philosophizes", fullProperNoun.Examples[0].English)

[<Fact>]
let ``ProperNoun emits case number order`` () =
    Assert.Equal<string[]>([| "nominative"; "singular" |], features fullProperNoun "Σωκράτης")

[<Fact>]
let ``ProperNoun emits forms in case then number order`` () =
    Assert.Equal<string[]>(
        [| "Σωκράτης"; "Σωκράτη"; "Σωκράτους"; "Σώκρατες" |],
        forms fullProperNoun
    )

[<Fact>]
let ``ProperNoun has empty forms when no declensions`` () =
    let entry = Convert.toInterop (ProperNoun(properNoun "Σωκράτης" [] None []))
    Assert.Empty(entry.Forms)

[<Fact>]
let ``Particle maps scalar fields`` () =
    let entry =
        Convert.toInterop (Particle(particle "μέν" [ "indeed" ] [ ex "ὁ μὲν" "he indeed" ]))

    Assert.Equal(PartOfSpeech.Particle, entry.Pos)
    Assert.Equal("μέν", entry.Lemma)
    Assert.Equal<string[]>([| "indeed" |], entry.Glosses)
    Assert.Equal("ὁ μὲν", (Assert.Single entry.Examples).Greek)
    Assert.Equal("he indeed", entry.Examples[0].English)

[<Fact>]
let ``Particle has no forms`` () =
    let entry = Convert.toInterop (Particle(particle "μέν" [] []))
    Assert.Empty(entry.Forms)

let private fullVerb =
    let firstSingular text = nf (Some text) None

    let indicativeActiveImperfective =
        tenseForms
            (personForms (firstSingular "λύνω") (firstSingular "λύνεις") (nf None None))
            (personForms (firstSingular "ἔλυον") (nf None None) (nf None None))
            (emptyPersonForms ())
            (emptyPersonForms ())
            (emptyPersonForms ())

    let indicativeActivePerfective =
        tenseForms
            (emptyPersonForms ())
            (emptyPersonForms ())
            (personForms (firstSingular "ἔλυσα") (nf None None) (nf None None))
            (emptyPersonForms ())
            (emptyPersonForms ())

    let indicativePassive =
        aspectForms
            (tenseForms
                (personForms (firstSingular "λύνομαι") (nf None None) (nf None None))
                (emptyPersonForms ())
                (emptyPersonForms ())
                (emptyPersonForms ())
                (emptyPersonForms ()))
            (emptyTenseForms ())

    let moods =
        moodForms
            (voiceForms
                (aspectForms indicativeActiveImperfective indicativeActivePerfective)
                indicativePassive)
            (emptyVoiceForms ())

    let participles =
        participleForms
            (participleVoiceForms
                (participleTenseForms (Some "λύων") (Some "λύσας") None)
                (participleTenseForms (Some "λυόμενος") None None))

    let infinitives = infinitiveForms (Some "λῦσαι") (Some "λυθῆναι")

    Convert.toInterop (
        Verb(
            verb
                "λύνω"
                [ "loosen" ]
                (verbForms moods participles infinitives)
                [ ex "λύω τὸν ἵππον" "I loosen the horse" ]
        )
    )

[<Fact>]
let ``Verb maps scalar fields`` () =
    Assert.Equal(PartOfSpeech.Verb, fullVerb.Pos)
    Assert.Equal("λύνω", fullVerb.Lemma)
    Assert.Equal<string[]>([| "loosen" |], fullVerb.Glosses)
    Assert.Equal("λύω τὸν ἵππον", (Assert.Single fullVerb.Examples).Greek)
    Assert.Equal("I loosen the horse", fullVerb.Examples[0].English)

[<Fact>]
let ``Verb emits finite feature order`` () =
    Assert.Equal<string[]>(
        [| "indicative"; "active"; "imperfective"; "present"; "first-person"; "singular" |],
        features fullVerb "λύνω"
    )

    Assert.Equal<string[]>(
        [| "indicative"; "active"; "imperfective"; "present"; "second-person"; "singular" |],
        features fullVerb "λύνεις"
    )

    Assert.Equal<string[]>(
        [| "indicative"; "active"; "perfective"; "past"; "first-person"; "singular" |],
        features fullVerb "ἔλυσα"
    )

    Assert.Equal<string[]>(
        [| "indicative"; "passive"; "imperfective"; "present"; "first-person"; "singular" |],
        features fullVerb "λύνομαι"
    )

[<Fact>]
let ``Verb emits participle feature order`` () =
    Assert.Equal<string[]>([| "participle"; "active"; "present" |], features fullVerb "λύων")
    Assert.Equal<string[]>([| "participle"; "passive"; "present" |], features fullVerb "λυόμενος")

[<Fact>]
let ``Verb emits infinitive feature order`` () =
    Assert.Equal<string[]>([| "infinitive"; "active"; "aorist" |], features fullVerb "λῦσαι")
    Assert.Equal<string[]>([| "infinitive"; "passive"; "aorist" |], features fullVerb "λυθῆναι")

[<Fact>]
let ``Verb emits finite participle infinitive order`` () =
    Assert.Equal<string[]>(
        [| "λύνω"; "λύνεις"; "ἔλυον"; "ἔλυσα"; "λύνομαι"
           "λύων"; "λύσας"; "λυόμενος"
           "λῦσαι"; "λυθῆναι" |],
        forms fullVerb
    )

[<Fact>]
let ``Verb has empty forms when no conjugations`` () =
    let entry = Convert.toInterop (Verb(verb "λύνω" [] (emptyVerbForms ()) []))
    Assert.Empty(entry.Forms)
