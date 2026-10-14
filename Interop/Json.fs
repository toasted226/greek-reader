namespace GreekReader.Interop

open System
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Json.Serialization
open System.Text.Json.Serialization.Metadata
open System.Text.Unicode

module Json =

    let private addPolymorphism (typeInfo: JsonTypeInfo) =
        if typeInfo.Type = typeof<AnalysedWord> then
            let polymorphism = JsonPolymorphismOptions()
            polymorphism.TypeDiscriminatorPropertyName <- "type"
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<NounWord>, "noun"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<DeterminerWord>, "determiner"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<VerbWord>, "verb"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<AdjectiveWord>, "adjective"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<AdverbWord>, "adverb"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<ArticleWord>, "article"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<PronounWord>, "pronoun"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<PrepositionWord>, "preposition"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<ConjunctionWord>, "conjunction"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<InterjectionWord>, "interjection"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<NumeralWord>, "numeral"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<ProperNounWord>, "properNoun"))
            polymorphism.DerivedTypes.Add(JsonDerivedType(typeof<ParticleWord>, "particle"))
            typeInfo.PolymorphismOptions <- polymorphism

    let options =
        let serializerOptions = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
        serializerOptions.Converters.Add(JsonStringEnumConverter(JsonNamingPolicy.CamelCase))
        serializerOptions.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
        serializerOptions.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
        let resolver = DefaultJsonTypeInfoResolver()
        resolver.Modifiers.Add(Action<JsonTypeInfo>(addPolymorphism))
        serializerOptions.TypeInfoResolver <- resolver
        serializerOptions

    let serialize (value: 'T) : string =
        JsonSerializer.Serialize<'T>(value, options)
