namespace GreekReader.Parsing

open GreekReader.Model
open GreekReader.Wiktionary

module VerbParser =
    
    let getForm (forms: Form list) : string option =
        match forms |> List.tryHead |> Option.map _.Form with
        | Some "-" -> None
        | Some s -> Some(s)
        | None -> None

    let getNumbers (forms: Form list) : NumberForms =
        { Singular = forms |> Utils.withTag "singular" |> getForm
          Plural = forms |> Utils.withTag "plural" |> getForm }

    let getPersons (forms: Form list) : PersonForms =
        { FirstPerson = forms |> Utils.withTag "first-person" |> getNumbers
          SecondPerson = forms |> Utils.withTag "second-person" |> getNumbers
          ThirdPerson = forms |> Utils.withTag "third-person" |> getNumbers }

    let getTenses (forms: Form list) : TenseForms =
        let hasTense =
            forms |> Utils.formsHaveTags [ "present"; "imperfect"; "past"; "future"; "dependent" ]

        { Present =
            if hasTense then
                forms |> Utils.withTag "present" |> getPersons
            else
                forms |> getPersons
          Imperfect = forms |> Utils.withTag "imperfect" |> getPersons
          Past = forms |> Utils.withTag "past" |> getPersons
          Future = forms |> Utils.withTag "future" |> getPersons
          Dependent = forms |> Utils.withTag "dependent" |> getPersons }

    let getAspects (forms: Form list) : AspectForms =
        { Imperfective = forms |> Utils.withTag "imperfective" |> getTenses
          Perfective = forms |> Utils.withTag "perfective" |> getTenses }

    let getVoices (forms: Form list) : VoiceForms =
        { Active = forms |> Utils.withTag "active" |> getAspects
          Passive = forms |> Utils.withTag "passive" |> getAspects }

    let getMoods (forms: Form list) : MoodForms =
        { Indicative = forms |> Utils.withTag "indicative" |> getVoices
          Imperative = forms |> Utils.withTag "imperative" |> getVoices }

    let getParticipleTenses (forms: Form list) : ParticipleTenseForms =
        { Present = forms |> Utils.withTag "present" |> List.tryHead |> Option.map _.Form
          Past = forms |> Utils.withTag "past" |> List.tryHead |> Option.map _.Form
          Perfect = forms |> Utils.withTag "perfect" |> List.tryHead |> Option.map _.Form }

    let getParticipleVoices (forms: Form list) : ParticipleVoiceForms =
        { Active = forms |> Utils.withTag "active" |> getParticipleTenses
          Passive = forms |> Utils.withTag "passive" |> getParticipleTenses }

    let getParticiples (forms: Form list) : ParticipleForms =
        { Voice = forms |> Utils.withTag "participle" |> getParticipleVoices }

    let getAoristInfinitives (forms: Form list) : InfinitiveForms =
        { Active = forms |> Utils.withTag "active" |> List.tryHead |> Option.map _.Form
          Passive = forms |> Utils.withTag "passive" |> List.tryHead |> Option.map _.Form }

    let getVerbForms (forms: Form list) : VerbForms =
        { Moods = getMoods forms
          Participles = getParticiples forms
          AoristInfinitives = forms |> Utils.withTag "infinitive-aorist" |> getAoristInfinitives }

    let analyse (w: Word) : AnalysedWord =
        Verb
            { Lemma = w.Word
              Glosses = w.Senses |> Option.defaultValue [] |> Utils.getGlosses
              Conjugations = getVerbForms (w.Forms |> Option.defaultValue []) }

    let analyseWords (words: Word seq) : AnalysedWord seq = words |> Seq.map analyse

    let analyseWordsList (words: Word list) : AnalysedWord list = words |> Seq.map analyse |> Seq.toList
