namespace TextOn.Atom.DTO

[<ReflectedDefinition>]
module DTO =
    type ParseRequest = { FileName : string; IsAsync : bool; Lines : string[]}
    type ProjectRequest = { FileName : string}
    type DeclarationsRequest = {FileName : string}
    type HelptextRequest = {Symbol : string}
    type PositionRequest = {FileName : string; Line : int; Column : int; Filter : string}
    type LintRequest = {FileName : string}
    type CompletionRequest = {FileName : string; SourceLine : string; Line : int; Column : int; Filter : string}
    type GeneratorStartRequest = {FileName : string; LineNumber : int; Lines : string[] }
    type GeneratorStopRequest = {Blank:string}
    type GenerateRequest = {Blank:string}
    type NavigateRequest = {FileName : string ; NavigateType : string ; Name : string }
    type GeneratorValueSetRequest = {Type:string;Name:string;Value:string}

    type OverloadSignature = {
        Signature: string
        Comment: string }

    type Error = {
        /// 1-indexed first line of the error block
        StartLine : int
        /// 1-indexed first column of the error block
        StartColumn : int
        /// 1-indexed last line of the error block
        EndLine : int
        /// 1-indexed last column of the error block
        EndColumn : int
        /// Description of the error
        Message : string
        /// The severity - "Error" or "Warning".
        Severity : string
        /// Type of the Error
        Subcategory : string }

    type Declaration = {
        File : string
        Line : int
        Column : int }

    type Completion = {
        Name : string
        ReplacementText: string
        Glyph : string
        GlyphChar: string
    }

    type SymbolUse = {
      Filename : string
      StartLine : int
      StartColumn : int
      EndLine : int
      EndColumn : int
      IsFromDefinition : bool
      IsFromAttribute : bool
      IsFromComputationExpression : bool
      IsFromDispatchSlotImplementation : bool
      IsFromPattern : bool
      IsFromType : bool }

    type SymbolUses = {
        Name : string
        Uses : SymbolUse array }

    type Helptext = {
        Name : string
        Overloads: OverloadSignature [] [] }

    type LintWarning =
        {
            /// Warning to display to the user.
            Info: string
            /// 1-indexed first line of the lint block
            StartLine : int
            /// 1-indexed first column of the lint block
            StartColumn : int
            /// 1-indexed last line of the lint block
            EndLine : int
            /// 1-indexed last column of the lint block
            EndColumn : int
            /// Entire input file, needed to display where in the file the error occurred.
            Input: string }

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
        Value : string }

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

    type Result<'T> = {Kind : string; Data : 'T}
    type HelptextResult = Result<Helptext>
    type CompletionResult = Result<Completion[]>
    type SymbolUseResult = Result<SymbolUses>
    type TooltipResult = Result<OverloadSignature[][]>
    type ParseResult = Result<Error[]>
    type FindDeclarationResult = Result<Declaration>
    type LintResult = Result<LintWarning[]>
    type GeneratorStartResult = Result<GeneratorData>
    type NavigateResult = Result<NavigateData>
