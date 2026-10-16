using Microsoft.FSharp.Core;
using M = GreekReader.Model;
using I = GreekReader.Interop;

namespace GreekReader.Tests;

public class ConvertTests
{
    [Fact]
    public void Noun_maps_every_field()
    {
        var word = Assert.IsType<I.NounWord>(I.Convert.toInterop(Fixtures.Noun()));

        Assert.Equal(I.PartOfSpeech.Noun, word.PartOfSpeech);
        Assert.Equal("λόγος", word.Lemma);
        Assert.Equal(new[] { "word", "reason" }, word.Glosses.ToArray());
        Assert.True(word.Gender.HasValue);
        Assert.Equal(I.Gender.Masculine, word.Gender!.Value);
        Assert.Equal("λόγος", word.Declensions.Nominative.Singular);
        Assert.Equal("λόγοι", word.Declensions.Nominative.Plural);
        Assert.Equal("λόγον", word.Declensions.Accusative.Singular);
        Assert.Equal("λόγων", word.Declensions.Genitive.Plural);
        Assert.Equal("λόγε", word.Declensions.Vocative.Singular);
        Assert.Equal("ὁ λόγος", Assert.Single(word.Examples).Greek);
        Assert.Equal("the word", word.Examples[0].English);
    }

    [Fact]
    public void Missing_leaves_map_to_null()
    {
        var noun = M.AnalysedWord.NewNoun(
            new M.NounInfo("ἄνθρωπος", Fixtures.List("human"), FSharpOption<M.Gender>.None, Fixtures.NoCases(), Fixtures.NoExamples()));

        var word = Assert.IsType<I.NounWord>(I.Convert.toInterop(noun));

        Assert.False(word.Gender.HasValue);
        Assert.Null(word.Declensions.Nominative.Singular);
        Assert.Null(word.Declensions.Accusative.Plural);
        Assert.Empty(word.Examples);
    }

    [Fact]
    public void Determiner_maps_gender_forms()
    {
        var info = new M.DeterminerInfo("ὁ", Fixtures.List("the"), Fixtures.GF(Fixtures.LogosCases(), Fixtures.NoCases(), Fixtures.NoCases()), Fixtures.FList(Fixtures.Ex("οὗτος ὁ ἀνήρ", "this man")));
        var word = Assert.IsType<I.DeterminerWord>(I.Convert.toInterop(M.AnalysedWord.NewDeterminer(info)));

        Assert.Equal(I.PartOfSpeech.Determiner, word.PartOfSpeech);
        Assert.Equal("λόγος", word.Declensions.Masculine.Nominative.Singular);
        Assert.Null(word.Declensions.Feminine.Nominative.Singular);
        Assert.Equal("this man", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Verb_maps_deeply_nested_forms()
    {
        var info = new M.VerbInfo("λύνω", Fixtures.List("loosen"), Fixtures.Verb("λύνω", "λύων", "λύσει"), Fixtures.FList(Fixtures.Ex("λύω τὸν ἵππον", "I loose the horse")));
        var word = Assert.IsType<I.VerbWord>(I.Convert.toInterop(M.AnalysedWord.NewVerb(info)));

        Assert.Equal(I.PartOfSpeech.Verb, word.PartOfSpeech);
        Assert.Equal("λύνω", word.Conjugations.Moods.Indicative.Active.Imperfective.Present.FirstPerson.Singular);
        Assert.Null(word.Conjugations.Moods.Indicative.Active.Imperfective.Present.ThirdPerson.Singular);
        Assert.Equal("λύων", word.Conjugations.Participles.Voice.Active.Present);
        Assert.Equal("λύσει", word.Conjugations.AoristInfinitives.Active);
        Assert.Null(word.Conjugations.Moods.Imperative.Active.Imperfective.Present.FirstPerson.Singular);
        Assert.Equal("I loose the horse", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Adjective_maps_degrees()
    {
        var degrees = new M.DegreeForms(Fixtures.GF(Fixtures.LogosCases(), Fixtures.NoCases(), Fixtures.NoCases()), Fixtures.NoGenders(), Fixtures.NoGenders());
        var info = new M.AdjectiveInfo("καλός", Fixtures.List("beautiful"), degrees, Fixtures.FList(Fixtures.Ex("καλὸς ἀνήρ", "a beautiful man")));
        var word = Assert.IsType<I.AdjectiveWord>(I.Convert.toInterop(M.AnalysedWord.NewAdjective(info)));

        Assert.Equal(I.PartOfSpeech.Adjective, word.PartOfSpeech);
        Assert.Equal("λόγος", word.Degrees.Positive.Masculine.Nominative.Singular);
        Assert.Null(word.Degrees.Comparative.Masculine.Nominative.Singular);
        Assert.Equal("a beautiful man", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Adverb_maps_degrees()
    {
        var info = new M.AdverbInfo("εὖ", Fixtures.List("well"), new M.AdverbDegrees(Fixtures.Opt("ἄμεινον"), null), Fixtures.FList(Fixtures.Ex("εὖ λέγει", "he speaks well")));
        var word = Assert.IsType<I.AdverbWord>(I.Convert.toInterop(M.AnalysedWord.NewAdverb(info)));

        Assert.Equal(I.PartOfSpeech.Adverb, word.PartOfSpeech);
        Assert.Equal("ἄμεινον", word.Degrees.Comparative);
        Assert.Null(word.Degrees.Superlative);
        Assert.Equal("he speaks well", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Article_maps_kind_and_inflections()
    {
        var info = new M.ArticleInfo("ὁ", Fixtures.List("the"), M.ArticleType.Definite, Fixtures.GF(Fixtures.LogosCases(), Fixtures.NoCases(), Fixtures.NoCases()), Fixtures.FList(Fixtures.Ex("ὁ ἀνήρ", "the man")));
        var word = Assert.IsType<I.ArticleWord>(I.Convert.toInterop(M.AnalysedWord.NewArticle(info)));

        Assert.Equal(I.PartOfSpeech.Article, word.PartOfSpeech);
        Assert.Equal(I.ArticleKind.Definite, word.Kind);
        Assert.Equal("λόγος", word.Inflections.Masculine.Nominative.Singular);
        Assert.Equal("the man", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Pronoun_maps_gender_declined()
    {
        var info = new M.PronounInfo("αὐτός", Fixtures.List("he"), M.PronounDeclensions.NewGenderDeclined(Fixtures.NoGenders()), Fixtures.FList(Fixtures.Ex("αὐτὸς λέγει", "he himself speaks")));
        var word = Assert.IsType<I.PronounWord>(I.Convert.toInterop(M.AnalysedWord.NewPronoun(info)));

        Assert.Equal(I.PartOfSpeech.Pronoun, word.PartOfSpeech);
        Assert.NotNull(word.GenderDeclensions);
        Assert.Null(word.CaseDeclensions);
        Assert.Equal("he himself speaks", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Pronoun_maps_non_gender_declined()
    {
        var info = new M.PronounInfo("τις", Fixtures.List("someone"), M.PronounDeclensions.NewNonGenderDeclined(Fixtures.LogosCases()), Fixtures.FList(Fixtures.Ex("τὶς λέγει", "someone speaks")));
        var word = Assert.IsType<I.PronounWord>(I.Convert.toInterop(M.AnalysedWord.NewPronoun(info)));

        Assert.Null(word.GenderDeclensions);
        Assert.NotNull(word.CaseDeclensions);
        Assert.Equal("λόγος", word.CaseDeclensions!.Nominative.Singular);
        Assert.Equal("someone speaks", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Preposition_maps_examples()
    {
        var info = new M.PrepositionInfo("ἐν", Fixtures.List("in"), Fixtures.FList(Fixtures.Ex("ἐν ἀρχῇ", "in the beginning")));
        var word = Assert.IsType<I.PrepositionWord>(I.Convert.toInterop(M.AnalysedWord.NewPreposition(info)));

        Assert.Equal(I.PartOfSpeech.Preposition, word.PartOfSpeech);
        Assert.Equal("ἐν ἀρχῇ", Assert.Single(word.Examples).Greek);
        Assert.Equal("in the beginning", word.Examples[0].English);
    }

    [Fact]
    public void Conjunction_maps_examples()
    {
        var info = new M.ConjunctionInfo("καί", Fixtures.List("and"), Fixtures.FList(Fixtures.Ex("α", "a")));
        var word = Assert.IsType<I.ConjunctionWord>(I.Convert.toInterop(M.AnalysedWord.NewConjunction(info)));

        Assert.Equal(I.PartOfSpeech.Conjunction, word.PartOfSpeech);
        Assert.Equal("a", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Interjection_maps_examples()
    {
        var info = new M.InterjectionInfo("οἴμοι", Fixtures.List("alas"), Fixtures.FList(Fixtures.Ex("α", "a")));
        var word = Assert.IsType<I.InterjectionWord>(I.Convert.toInterop(M.AnalysedWord.NewInterjection(info)));

        Assert.Equal(I.PartOfSpeech.Interjection, word.PartOfSpeech);
        Assert.Equal("α", Assert.Single(word.Examples).Greek);
    }

    [Fact]
    public void Numeral_maps_declensions_when_present()
    {
        var cases = new M.NumeralCases(Fixtures.Opt("εἷς"), Fixtures.Opt("ἕνα"), null, null);
        var genders = new M.NumeralGenders(cases, cases, cases);
        var info = new M.NumeralInfo("εἷς", Fixtures.List("one"), Fixtures.Some(genders), Fixtures.FList(Fixtures.Ex("εἷς ἀνήρ", "one man")));
        var word = Assert.IsType<I.NumeralWord>(I.Convert.toInterop(M.AnalysedWord.NewNumeral(info)));

        Assert.Equal(I.PartOfSpeech.Numeral, word.PartOfSpeech);
        Assert.NotNull(word.Declensions);
        Assert.Equal("εἷς", word.Declensions!.Masculine.Nominative);
        Assert.Null(word.Declensions.Masculine.Genitive);
        Assert.Equal("one man", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void Numeral_without_declensions_maps_null()
    {
        var info = new M.NumeralInfo("πέντε", Fixtures.List("five"), FSharpOption<M.NumeralGenders>.None, Fixtures.NoExamples());
        var word = Assert.IsType<I.NumeralWord>(I.Convert.toInterop(M.AnalysedWord.NewNumeral(info)));

        Assert.Null(word.Declensions);
    }

    [Fact]
    public void ProperNoun_maps_declensions_when_present()
    {
        var info = new M.ProperNounInfo("Σωκράτης", Fixtures.List("Socrates"), Fixtures.Some(Fixtures.LogosCases()), Fixtures.FList(Fixtures.Ex("Σωκράτης φιλόσοφος", "Socrates the philosopher")));
        var word = Assert.IsType<I.ProperNounWord>(I.Convert.toInterop(M.AnalysedWord.NewProperNoun(info)));

        Assert.Equal(I.PartOfSpeech.ProperNoun, word.PartOfSpeech);
        Assert.Equal("λόγος", word.Declensions!.Nominative.Singular);
        Assert.Equal("Socrates the philosopher", Assert.Single(word.Examples).English);
    }

    [Fact]
    public void ProperNoun_without_declensions_maps_null()
    {
        var info = new M.ProperNounInfo("Ἀθῆναι", Fixtures.List("Athens"), FSharpOption<M.CaseForms>.None, Fixtures.NoExamples());
        var word = Assert.IsType<I.ProperNounWord>(I.Convert.toInterop(M.AnalysedWord.NewProperNoun(info)));

        Assert.Null(word.Declensions);
    }

    [Fact]
    public void Particle_maps_examples()
    {
        var word = Assert.IsType<I.ParticleWord>(I.Convert.toInterop(Fixtures.Particle()));

        Assert.Equal(I.PartOfSpeech.Particle, word.PartOfSpeech);
        Assert.Equal("δέ", word.Lemma);
        Assert.Equal("but", Assert.Single(word.Glosses));
        Assert.Equal("α", Assert.Single(word.Examples).Greek);
    }
}
