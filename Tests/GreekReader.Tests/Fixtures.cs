using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using M = GreekReader.Model;
using I = GreekReader.Interop;

namespace GreekReader.Tests;

internal static class Fixtures
{
    public static FSharpList<T> FList<T>(params T[] items) => ListModule.OfArray(items);

    public static FSharpList<string> List(params string[] items) => FList(items);

    public static FSharpOption<string> Opt(string? value) =>
        value is null ? FSharpOption<string>.None : FSharpOption<string>.Some(value);

    public static FSharpOption<T> Some<T>(T value) => FSharpOption<T>.Some(value);

    public static FSharpOption<M.Gender> Opt(M.Gender value) => FSharpOption<M.Gender>.Some(value);

    public static M.NumberForms NF(string? singular, string? plural) => new(Opt(singular), Opt(plural));

    public static M.CaseForms CF(M.NumberForms nominative, M.NumberForms accusative, M.NumberForms genitive, M.NumberForms vocative)
        => new(nominative, accusative, genitive, vocative);

    public static M.CaseForms NoCases() => CF(NF(null, null), NF(null, null), NF(null, null), NF(null, null));

    public static M.GenderForms GF(M.CaseForms masculine, M.CaseForms feminine, M.CaseForms neuter)
        => new(masculine, feminine, neuter);

    public static M.GenderForms NoGenders() => GF(NoCases(), NoCases(), NoCases());

    public static M.PersonForms PF(M.NumberForms first, M.NumberForms second, M.NumberForms third) => new(first, second, third);

    public static M.PersonForms NoPersons() => PF(NF(null, null), NF(null, null), NF(null, null));

    public static M.TenseForms TF(M.PersonForms present, M.PersonForms imperfect, M.PersonForms past, M.PersonForms future, M.PersonForms dependent)
        => new(present, imperfect, past, future, dependent);

    public static M.TenseForms NoTenses() => TF(NoPersons(), NoPersons(), NoPersons(), NoPersons(), NoPersons());

    public static M.AspectForms AF(M.TenseForms imperfective, M.TenseForms perfective) => new(imperfective, perfective);

    public static M.AspectForms NoAspects() => AF(NoTenses(), NoTenses());

    public static M.VoiceForms VF(M.AspectForms active, M.AspectForms passive) => new(active, passive);

    public static M.VoiceForms NoVoices() => VF(NoAspects(), NoAspects());

    public static M.MoodForms MF(M.VoiceForms indicative, M.VoiceForms imperative) => new(indicative, imperative);

    public static M.MoodForms NoMoods() => MF(NoVoices(), NoVoices());

    public static M.Example Ex(string greek, string english) => new(greek, english);

    public static FSharpList<M.Example> NoExamples() => FList<M.Example>();

    public static M.ParticipleTenseForms PPartTenses(string? present, string? past, string? perfect)
        => new(Opt(present), Opt(past), Opt(perfect));

    public static M.VerbForms Verb(string present1Sg, string participle, string infinitive)
    {
        var firstPerson = PF(NF(present1Sg, null), NF(null, null), NF(null, null));
        var present = TF(firstPerson, NoPersons(), NoPersons(), NoPersons(), NoPersons());
        var imperfective = AF(present, NoTenses());
        var active = VF(imperfective, NoAspects());
        var indicative = MF(active, NoVoices());

        var participleVoices = new M.ParticipleVoiceForms(PPartTenses(participle, null, null), PPartTenses(null, null, null));
        var participles = new M.ParticipleForms(participleVoices);

        var infinitives = new M.InfinitiveForms(Opt(infinitive), null);

        return new M.VerbForms(indicative, participles, infinitives);
    }

    public static M.CaseForms LogosCases() => CF(
        NF("λόγος", "λόγοι"),
        NF("λόγον", "λόγους"),
        NF("λόγου", "λόγων"),
        NF("λόγε", "λόγοι"));

    public static M.AnalysedWord Noun() =>
        M.AnalysedWord.NewNoun(new M.NounInfo("λόγος", List("word", "reason"), Opt(M.Gender.Masculine), LogosCases(), FList(Ex("ὁ λόγος", "the word"))));

    public static M.AnalysedWord Particle() =>
        M.AnalysedWord.NewParticle(new M.ParticleInfo("δέ", List("but"), FList(Ex("α", "a"))));
}
