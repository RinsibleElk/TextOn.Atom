﻿namespace TextOn.Atom.DTO

[<ReflectedDefinition>]
module DTO =
    type ParseRequest = { FileName : string; Lines : string[] }
    type GeneratorStartRequest =
        {
            FileName : string
            LineNumber : int
            Lines : string[]
        }
    type GeneratorStopRequest = { Blank : string }
    type BrowserStartRequest =
        {
            FileName : string
            LineNumber : int
            Lines : string[]
        }
    type BrowserExpandRequest = { browserFile : string ; rootFunction : string ; indexPath : int[] }
    type BrowserStopRequest = { Blank : string }
    type GeneratorConfiguration =
        {
            NumSpacesBetweenSentences : int
            NumBlankLinesBetweenParagraphs : int
            WindowsLineEndings : bool
        }
    type BrowserCycleRequest =
        {
            FileName : string
            // First line is 1
            LineNumber : int
        }
    type GenerateRequest = {Config:GeneratorConfiguration}
    type NavigateRequest = { FileName : string ; NavigateType : string ; Name : string }
    type GeneratorValueSetRequest = { Type : string ; Name : string ; Value : string }
    type BrowserValueSetRequest = { FileName : string ; Type : string ; Name : string ; Value : string }
    type UpdateGeneratorRequest = { Blank : string }
    type UpdateBrowserRequest = { SelectedIdentity : string }
    type SuggestionRequest = { fileName : string ; ``type`` : string ; line : string ; column : int } // type will be "Function", "Attribute", "Variable" or "QuotedString"
    type NavigateToSymbolRequest = { FileName : string ; Line : string ; Column : int }

    type Error = {
        // OPS It's weird that I am using Points here no?
        // { { Start line (0-based), Start column (0-based) }, { End line (0-based), End column (1-based) } }
        range : float [] []
        /// Description of the error
        text : string
        /// Type of the Error
        ``type`` : string
        // The file.
        filePath : string }

    type GeneratorInput =
        {
            name : string
            text : string
            value : string
            items : string[]
            permitsFreeValue : bool
        }

    type OutputString = {
        File : string
        LineNumber : int
        Value : string
        IsParagraphBreak : bool }

    type GeneratorData =
        {
            fileName : string
            functionName : string
            attributes : GeneratorInput[]
            variables : GeneratorInput[]
            canGenerate : bool
            output : OutputString[]
        }
    type BrowserNode =
        {
            text : string
            nodeType : string
            rootFunction : string
            indexPath : int[]
            isCollapsible : bool
            isCollapsed : bool
            file : string
            line : int
            children : BrowserNode[]
        }
    type BrowserUpdate =
        {
            attributes : GeneratorInput[]
            variables : GeneratorInput[]
            nodes : BrowserNode[]
            file : string
            selectedPath : int[]
        }
    type BrowserItems =
        {
            newItems : BrowserNode[]
        }
    
    type NavigateData =
        {
            FileName : string
            LineNumber : int
            Location : int
        }
    type Suggestion = { text : string ; ``type`` : string ; description : string }

    type Result<'T> = { Kind : string ; Data : 'T[] }
    type ParseResult = Result<Error>
    type GeneratorStartResult = Result<GeneratorData>
    type NavigateResult = Result<NavigateData>
    type SuggestionResult = Result<Suggestion>
    type BrowserUpdateResult = Result<BrowserUpdate>