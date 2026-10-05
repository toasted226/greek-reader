namespace GreekReader.Wiktionary

open System.IO
open System.Text.Json

module Jsonl =

  let private options =
    JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

  let parseLines<'T> (filepath: string) : 'T seq =
    File.ReadLines filepath
    |> Seq.map (fun line -> JsonSerializer.Deserialize<'T>(line, options))

  let parseLinesList<'T> (filepath: string) : 'T list =
    parseLines<'T> filepath |> Seq.toList

  let parseWords (filepath: string) : Word seq =
    parseLines<Word> filepath

  let parseWordsList (filepath: string) : Word list =
    parseWords filepath |> Seq.toList