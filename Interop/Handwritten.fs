namespace GreekReader.Interop

module Handwritten =

    let private form (features: string list) (spellings: string list) : FormEntry list =
        spellings
        |> List.map (fun s ->
            { Form = s
              Features = List.toArray features })

    /// Builds an entry and always adds the headword as a form of itself,
    /// so the lemma is findable even when it has no other forms.
    let private entry pos lemma glosses (forms: FormEntry list) : LemmaEntry =
        { Lemma = lemma
          Pos = pos
          Glosses = List.toArray glosses
          Examples = [||]
          Forms =
            { Form = lemma
              Features = [| "lemma" |] }
            :: forms
            |> List.distinctBy (fun f -> f.Form, f.Features)
            |> List.toArray }

    let private sg1 = [ "first-person"; "singular" ]
    let private sg2 = [ "second-person"; "singular" ]

    let private pl1 = [ "first-person"; "plural" ]
    let private pl2 = [ "second-person"; "plural" ]

    let pronouns: LemmaEntry list =
        [ entry
              PartOfSpeech.Pronoun
              "εγώ"
              [ "I" ]
              (form (sg1 @ [ "strong"; "nominative" ]) [ "εγώ"; "’γώ" ]
               @ form (sg1 @ [ "strong"; "accusative" ]) [ "εμένα"; "μένα" ]
               @ form (sg1 @ [ "weak"; "accusative" ]) [ "με"; "μ’" ]
               @ form (sg1 @ [ "weak"; "genitive" ]) [ "μου" ])

          entry
              PartOfSpeech.Pronoun
              "εσύ"
              [ "you (singular)" ]
              (form (sg2 @ [ "strong"; "nominative" ]) [ "εσύ"; "’σύ" ]
               @ form (sg2 @ [ "strong"; "accusative" ]) [ "εσένα"; "σένα" ]
               @ form (sg2 @ [ "weak"; "accusative" ]) [ "σε"; "σ’" ]
               @ form (sg2 @ [ "weak"; "genitive" ]) [ "σου" ])

          entry
              PartOfSpeech.Pronoun
              "εμείς"
              [ "we" ]
              (form (pl1 @ [ "strong"; "nominative" ]) [ "εμείς" ]
               @ form (pl1 @ [ "strong"; "accusative" ]) [ "εμάς" ]
               @ form (pl1 @ [ "weak"; "accusative" ]) [ "μας" ]
               @ form (pl1 @ [ "weak"; "genitive" ]) [ "μας" ])


          entry
              PartOfSpeech.Pronoun
              "εσείς"
              [ "you (plural)" ]
              (form (pl1 @ [ "strong"; "nominative" ]) [ "εσείς" ]
               @ form (pl1 @ [ "strong"; "accusative" ]) [ "εσάς" ]
               @ form (pl1 @ [ "weak"; "accusative" ]) [ "σας" ]
               @ form (pl1 @ [ "weak"; "genitive" ]) [ "σας" ]) ]

    let indeclinables: LemmaEntry list =
        [ entry PartOfSpeech.Adjective "μπλε" [ "blue" ] []
          entry PartOfSpeech.Adjective "ροζ" [ "pink" ] [] ]

    let all = pronouns @ indeclinables

    let merge (handwritten: LemmaEntry list) (extracted: LemmaEntry seq) : LemmaEntry seq =
        let key (e: LemmaEntry) = e.Lemma, e.Pos
        let overridden = handwritten |> List.map key |> Set.ofList

        extracted
        |> Seq.filter (fun e -> not (overridden.Contains(key e)))
        |> fun rest -> Seq.append rest handwritten