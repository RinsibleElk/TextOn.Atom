﻿namespace TextOn.Atom.Js

[<ReflectedDefinition>]
module DTO =
    type ParseRequest = { FileName : string; IsAsync : bool; Lines : string[]}
    type ProjectRequest = { FileName : string}
    type DeclarationsRequest = {FileName : string}
    type HelptextRequest = {Symbol : string}
    type PositionRequest = {FileName : string; Line : int; Column : int; Filter : string}
    type LintRequest = {FileName : string}
    type CompletionRequest = {FileName : string; SourceLine : string; Line : int; Column : int; Filter : string}

    type OverloadSignature = {
        Signature: string
        Comment: string
    }

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
        Subcategory : string
        }

    type Declaration = {
        File : string
        Line : int
        Column : int
    }

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
      IsFromType : bool
    }

    type SymbolUses = {
        Name : string
        Uses : SymbolUse array
    }

    type Helptext = {
        Name : string
        Overloads: OverloadSignature [] []
    }

    type CompilerLocation = {
        Fcs : string
        Fsi : string
        MSBuild : string
    }

    type Range = {
        StartColumn: int
        StartLine: int
        EndColumn: int
        EndLine: int
    }

    type Lint = {
        Info : string
        Input : string
        Range : Range
    }

    type Result<'T> = {Kind : string; Data : 'T}
    type CompilerLocationResult = Result<CompilerLocation>
    type HelptextResult = Result<Helptext>
    type CompletionResult = Result<Completion[]>
    type SymbolUseResult = Result<SymbolUses>
    type TooltipResult = Result<OverloadSignature[][]>
    type ParseResult = Result<Error[]>
    type FindDeclarationResult = Result<Declaration>
    type LintResult = Result<Lint[]>
