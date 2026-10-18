namespace GreekReader.Interop

open GreekReader

module Api =

    [<CompiledName("Load")>]
    let load () : LemmaEntry[] =
        API.partsOfSpeech
        |> API.parseWiktionaryData
        |> API.analyseWiktionaryData
        |> Seq.map Convert.toInterop
        |> Seq.toArray
    
    [<CompiledName("Normalise")>]
    let normalise (s: string) : string =
        GreekReader.Parsing.Text.normalise s

    [<CompiledName("Tokenise")>]
    let tokenise (s: string) =
        GreekReader.Parsing.Text.tokenise s
        |> List.toArray