namespace GreekReader.Interop

open System.Collections.Generic

type IGreekAnalyzer =
    abstract Query: string -> IReadOnlyList<AnalysedWord>
    abstract QueryText: string -> IReadOnlyDictionary<string, IReadOnlyList<AnalysedWord>>

type GreekAnalyzer() =

    member _.Query(query: string) : IReadOnlyList<AnalysedWord> =
        GreekReader.API.query query
        |> List.map Convert.toInterop
        |> List.toArray
        :> IReadOnlyList<AnalysedWord>

    member _.QueryText(text: string) : IReadOnlyDictionary<string, IReadOnlyList<AnalysedWord>> =
        let results = Dictionary<string, IReadOnlyList<AnalysedWord>>()
        for KeyValue(token, words) in GreekReader.API.queryText text do
            results[token] <- (words |> List.map Convert.toInterop |> List.toArray :> IReadOnlyList<AnalysedWord>)
        results :> IReadOnlyDictionary<string, IReadOnlyList<AnalysedWord>>

    interface IGreekAnalyzer with
        member this.Query(query: string) = this.Query query
        member this.QueryText(text: string) = this.QueryText text
