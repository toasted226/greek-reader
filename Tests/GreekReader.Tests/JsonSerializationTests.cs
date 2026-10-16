using System.Text.Json;
using I = GreekReader.Interop;

namespace GreekReader.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void Noun_serializes_with_discriminator_and_camel_case_enum()
    {
        var json = I.Json.serialize(I.Convert.toInterop(Fixtures.Noun()));

        Assert.Contains("\"type\":\"noun\"", json);
        Assert.Contains("\"partOfSpeech\":\"noun\"", json);
        Assert.Contains("\"gender\":\"masculine\"", json);
    }

    [Fact]
    public void Greek_text_is_not_unicode_escaped()
    {
        var json = I.Json.serialize(I.Convert.toInterop(Fixtures.Noun()));

        Assert.Contains("λόγος", json);
        Assert.DoesNotContain("\\u", json);
    }

    [Fact]
    public void Null_leaves_are_omitted()
    {
        var json = I.Json.serialize(I.Convert.toInterop(Fixtures.Particle()));

        Assert.DoesNotContain("null", json);
    }

    [Fact]
    public void Heterogeneous_collection_serializes_each_element_with_its_discriminator()
    {
        var words = new List<I.AnalysedWord>
        {
            I.Convert.toInterop(Fixtures.Noun()),
            I.Convert.toInterop(Fixtures.Particle()),
        };

        var json = I.Json.serialize<IReadOnlyList<I.AnalysedWord>>(words);

        Assert.Contains("\"type\":\"noun\"", json);
        Assert.Contains("\"type\":\"particle\"", json);
    }

    [Fact]
    public void Examples_are_serialized_for_every_part_of_speech()
    {
        var json = I.Json.serialize(I.Convert.toInterop(Fixtures.Noun()));

        Assert.Contains("\"examples\"", json);
        Assert.Contains("\"greek\":\"ὁ λόγος\"", json);
        Assert.Contains("\"english\":\"the word\"", json);
    }

    [Fact]
    public void Round_trips_through_System_Text_Json_without_polymorphic_metadata()
    {
        var json = I.Json.serialize(I.Convert.toInterop(Fixtures.Noun()));

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal("noun", root.GetProperty("type").GetString());
        Assert.Equal("λόγος", root.GetProperty("lemma").GetString());
        Assert.Equal("masculine", root.GetProperty("gender").GetString());
        Assert.Equal("λόγος", root.GetProperty("declensions").GetProperty("nominative").GetProperty("singular").GetString());
    }
}
