namespace GreekReader.Wiktionary

open System.IO
open System.Text.Json

module Jsonl =

  let private options =
    JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

  let parseLines<'T> (filepath: string) : 'T list =
    File.ReadLines filepath
    |> Seq.map (fun line -> JsonSerializer.Deserialize<'T>(line, options))
    |> Seq.toList

  let parseWords (filepath: string) : Word list =
    parseLines<Word> filepath