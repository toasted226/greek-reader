namespace GreekReader.Parsing

open System
open System.Globalization
open System.Text

module Text =

    type Token =
        { Text: string
          Start: int
          IsWord: bool }

    let normalise (s: string) : string =
        let sb = StringBuilder(s.Length)

        for c in s.Trim().Normalize(NormalizationForm.FormD) do
            if CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark then
                match Char.ToLowerInvariant(c) with
                | 'ς' -> sb.Append('σ') |> ignore
                | lc -> sb.Append(lc) |> ignore
                
        sb.ToString().Normalize(NormalizationForm.FormC)
 
    let private isApostrophe (c: char) =
        c = '\'' || c = '’' || c = 'ʼ' || c = '᾽'

    let private isWordChar (c: char) =
        not (isApostrophe c)
        && (Char.IsLetter c
            || CharUnicodeInfo.GetUnicodeCategory c = UnicodeCategory.NonSpacingMark)

    let tokenise (text: string) : Token list =
        let flush buf start isWord acc =
            match buf with
            | [] -> acc
            | _ ->
                { Text = String(buf |> List.rev |> Array.ofList)
                  Start = start
                  IsWord = isWord }
                :: acc

        let rec go chars i start buf isWord acc =
            match chars with
            | [] -> flush buf start isWord acc |> List.rev
            | c :: rest ->
                // buf is reversed, so its head is the last character consumed
                let closed =
                    isWord
                    && (match buf with
                        | last :: _ -> isApostrophe last
                        | [] -> false)

                let inWord = isWord && not (List.isEmpty buf) && not closed
                let w = isWordChar c || (isApostrophe c && inWord)

                if List.isEmpty buf then
                    go rest (i + 1) i [ c ] w acc
                elif w = isWord && not closed then
                    go rest (i + 1) start (c :: buf) isWord acc
                else
                    go rest (i + 1) i [ c ] w (flush buf start isWord acc)

        go (List.ofSeq text) 0 0 [] false []

    let private superscripts = [| '¹'; '²'; '³'; '⁴'; '⁵'; '*' |]
    let private wrappers = [| '['; ']'; '{'; '}'; '('; ')' |]

    let private clean (s: string) =
        s.Trim().Trim(wrappers).TrimEnd(superscripts).Trim(wrappers).Trim()

    let private isWord (s: string) =
        s <> "" && s |> Seq.forall (fun c -> Char.IsLetter c || c = '\'' || c = '’')

    let parseCell (cell: string) : string[] =
        cell.Split([| " - "; ","; ";"; " / " |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map clean
        |> Array.filter isWord
        |> Array.distinct