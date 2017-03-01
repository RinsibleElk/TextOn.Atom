namespace TextOn.Atom.DTO

[<ReflectedDefinition>]
module DTO =
    type ParseRequest = { FileName : string; Lines : string[] }
    type GeneratorStartRequest = { FileName : string; LineNumber : int; Lines : string[] }
    type GeneratorStopRequest = { Blank : string }
    type GeneratorConfiguration =
        {
            NumSpacesBetweenSentences : int
            NumBlankLinesBetweenParagraphs : int
            WindowsLineEndings : bool
        }
    type GenerateRequest = {Config:GeneratorConfiguration}
    type NavigateRequest = { FileName : string ; NavigateType : string ; Name : string }
    type GeneratorValueSetRequest = { Type : string ; Name : string ; Value : string }
    type UpdateGeneratorRequest = { Blank : string }

    type Error = {
        // { { Start line (0-based), Start column (0-based) }, { End line (0-based), End column (1-based) } }
        range : float [] []
        /// Description of the error
        text : string
        /// Type of the Error
        ``type`` : string
        // The file.
        filePath : string }

    type GeneratorAttribute =
        {
            Name : string
            Value : string
            Suggestions : string[]
            IsEditable : bool
        }
    type GeneratorVariable =
        {
            Name : string
            Text : string
            Value : string
            Suggestions : string[]
            IsEditable : bool
            IsFree : bool
        }

    type OutputString = {
        File : string
        LineNumber : int
        Value : string
        IsParagraphBreak : bool }

    type GeneratorData =
        {
            FileName : string
            FunctionName : string
            Attributes : GeneratorAttribute[]
            Variables : GeneratorVariable[]
            CanGenerate : bool
            Output : OutputString[]
        }

    type NavigateData =
        {
            FileName : string
            LineNumber : int
            Location : int
        }

    type Result<'T> = { Kind : string ; Data : 'T[] }
    type ParseResult = Result<Error>
    type GeneratorStartResult = Result<GeneratorData>
    type NavigateResult = Result<NavigateData>
