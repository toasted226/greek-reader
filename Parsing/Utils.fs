namespace GreekReader.Parsing

module Utils =
    
    let rec pickGender acc tags =
      match tags with
      | [] -> None
      | "masculine" :: _ when Option.isNone acc -> Some("masculine")
      | "feminine" :: _ when Option.isNone acc -> Some("feminine")
      | "neuter" :: _ when Option.isNone acc -> Some("neuter")
      | _ :: rest -> pickGender acc rest
    
    let rec pickCase acc tags =
      match tags with
      | [] -> None
      | "nominative" :: _ when Option.isNone acc -> Some("nominative")
      | "accusative" :: _ when Option.isNone acc -> Some("accusative")
      | "genitive" :: _ when Option.isNone acc -> Some("genitive")
      | "vocative" :: _ when Option.isNone acc -> Some("vocative")
      | _ :: rest -> pickCase acc rest
      
    let rec pickNumber acc tags =
      match tags with
      | [] -> None
      | "singular" :: _ when Option.isNone acc -> Some("singular")
      | "plural" :: _ when Option.isNone acc -> Some("plural")
      | _ :: rest -> pickNumber acc rest