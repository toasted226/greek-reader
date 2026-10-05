namespace GreekReader.Model

// One case per part of speech. Every NounInfo in Model/Noun.fs must be listed here,
// and this file must be compiled after every per-POS model file.
type AnalysedWord =
  | Noun of NounInfo