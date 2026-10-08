namespace GreekReader.Parsing

open System.Linq
open GreekReader.Model
open GreekReader.Wiktionary

module VerbParser =

    /// returns true if at least one of the provided tags are
    /// present in one of the given forms
    let hasTags (tags: string list) (forms: Form list) : bool =
        forms
        |> List.exists (fun f ->
            tags
            |> List.exists (fun t -> List.contains t (f.Tags |> Option.defaultValue [])))

    let withTag (tag: string) (forms: Form list) : Form list =
        forms |> List.filter (fun f -> (f.Tags |> Option.defaultValue []).Contains(tag))

    let getNumbers (forms: Form list) : NumberForms =
        { Singular = forms |> withTag "singular" |> List.tryHead |> Option.map _.Form
          Plural = forms |> withTag "plural" |> List.tryHead |> Option.map _.Form }

    let getPersons (forms: Form list) : PersonForms =
        { FirstPerson = forms |> withTag "first-person" |> getNumbers
          SecondPerson = forms |> withTag "second-person" |> getNumbers
          ThirdPerson = forms |> withTag "third-person" |> getNumbers }

    let getTenses (forms: Form list) : TenseForms =
        let hasTense =
            forms |> hasTags [ "present"; "imperfect"; "past"; "future"; "dependent" ]

        { Present =
            if hasTense then
                forms |> withTag "present" |> getPersons
            else
                forms |> getPersons
          Imperfect = forms |> withTag "imperfect" |> getPersons
          Past = forms |> withTag "past" |> getPersons
          Future = forms |> withTag "future" |> getPersons
          Dependent = forms |> withTag "dependent" |> getPersons }

    let getAspects (forms: Form list) : AspectForms =
        { Imperfective = forms |> withTag "imperfective" |> getTenses
          Perfective = forms |> withTag "perfective" |> getTenses }

    let getVoices (forms: Form list) : VoiceForms =
        { Active = forms |> withTag "active" |> getAspects
          Passive = forms |> withTag "passive" |> getAspects }

    let getMoods (forms: Form list) : MoodForms =
        { Indicative = forms |> withTag "indicative" |> getVoices
          Imperative = forms |> withTag "imperative" |> getVoices }

    let getParticiples (forms: Form list) : ParticipleForms = failwith "participles unimplemented"

    let getAoristInfinitives (forms: Form list) : InfinitiveForms =
        failwith "aorist infinitives unimplemented"

    let getVerbForms (forms: Form list) : VerbForms =
        { Moods = getMoods forms
          Participles = getParticiples forms
          AoristInfinitives = getAoristInfinitives forms }

    let analyse (w: Word) : AnalysedWord =
        Verb
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Conjugations = getVerbForms (w.Forms |> Option.defaultValue []) }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
