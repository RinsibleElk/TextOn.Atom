namespace TextOn.Atom

/// Store a mapping.
type StringNameValuePair = {
    /// The name. Case sensitive.
    Name : string
    /// The value. Case sensitive.
    Value : string }

/// What line ending character(s) to use.
type LineEnding =
    /// Windows-style (\r\n).
    | CRLF
    /// UNIX-style (\n).
    | LF

/// Configuration for things like line endings.
type GeneratorConfig = {
    /// Number of spaces between sentences.
    NumSpacesBetweenSentences : int
    /// Number of blank lines between paragraphs.
    NumBlankLinesBetweenParagraphs : int
    /// What line ending character(s) to use.
    LineEnding : LineEnding }

/// Used to configure the generator for a run with a given random seed.
type RandomSeed =
    /// Create a new random seed for this run.
    | NoSeed
    /// If known, the last used value will be reused again to generate the same values. If not known, then NoSeed will be used instead.
    | UseLastIfPossible
    /// Use a specific value for the seed.
    | SpecificValue of int

/// JSON inputs to generation.
type GeneratorInputs = {
    /// The random seed to use.
    RandomSeed : RandomSeed
    /// Generator configuration.
    Config : GeneratorConfig
    /// Attribute values.
    Attributes : StringNameValuePair list
    /// Variable values.
    Variables : StringNameValuePair list }

