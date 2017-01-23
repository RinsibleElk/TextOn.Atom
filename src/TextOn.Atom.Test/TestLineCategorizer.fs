﻿module TextOn.Atom.Test.TestLineCategorizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

let test s e =
    let r = LineCategorizer.categorize s |> Seq.toList
    let rLen = r |> List.length
    let eLen = e |> List.length
    if (rLen > eLen) then
        failwithf "Results too long - first unexpected line was %A" (r |> List.skip eLen |> List.head)
    else if (rLen < eLen) then
        failwithf "Results too short - first missed line was %A" (e |> List.skip rLen |> List.head)
    else
        List.zip r e
        |> List.tryFind
            (fun (r,e) ->
                if r.Category <> e.Category then true
                else if r.Index <> e.Index then true
                else if r.StartLine <> e.StartLine then true
                else if r.EndLine <> e.EndLine then true
                else if r.File <> e.File then true
                else (r.Lines |> List.ofSeq) <> (e.Lines |> List.ofSeq))
        |> Option.map (fun (r,e) -> sprintf "\n%A\n\n!=\n\n%A" e r)
        |> Option.iter (fun s -> failwithf "The lists didn't match - first failing line was: %s" s)
let exampleFileName = "example.texton"

[<Test>]
let ``Categorize empty file``() =
    test [] []

[<Test>]
let ``Categorize preprocessor error``() =
    test
        [
            {
                TopLevelFileLineNumber = 1
                CurrentFileLineNumber = 1
                CurrentFile = exampleFileName
                Contents =
                    Error
                        {
                            StartLocation = 1
                            EndLocation = 5
                            ErrorText = "Some error text"
                        }
            }
        ]
        [
            {
                Category = PreprocessorError
                Index = 0
                File = "example.texton"
                StartLine = 1
                EndLine = 1
                Lines =
                    Seq.singleton
                        {
                            TopLevelFileLineNumber = 1
                            CurrentFileLineNumber = 1
                            CurrentFile = "example.texton"
                            Contents =
                                Error {
                                    StartLocation = 1
                                    EndLocation = 5
                                    ErrorText = "Some error text" }
                    }
            }
        ]

[<Test>]
let ``Categorize preprocessor warning``() =
    test
        [
            {
                TopLevelFileLineNumber = 1
                CurrentFileLineNumber = 1
                CurrentFile = exampleFileName
                Contents =
                    Warning
                        {
                            StartLocation = 1
                            EndLocation = 5
                            WarningText = "Some warning text"
                        }
            }
        ]
        [
            {
                Category = PreprocessorWarning
                Index = 0
                File = "example.texton"
                StartLine = 1
                EndLine = 1
                Lines =
                    Seq.singleton
                        {
                            TopLevelFileLineNumber = 1
                            CurrentFileLineNumber = 1
                            CurrentFile = "example.texton"
                            Contents =
                                Warning {
                                    StartLocation = 1
                                    EndLocation = 5
                                    WarningText = "Some warning text" }
                    }
            }
        ]

let mainFunction =
    [
        "@func @main()"
        "{"
        "  @seq {"
        "    You are a bloke. [%Gender = \"Male\"]"
        "    You live in {$City|a city in $Country}."
        "    We are in $City which is in $Country."
        "    @break"
        "    @guyStuff() [%Gender = \"Male\"]"
        "  }"
        "}"
    ]

[<Test>]
let ``Categorize a single main function``() =
    let mainFunctionLines =
        mainFunction
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine, lastLine = mainFunctionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        mainFunctionLines
        [
            {
                Category = FuncDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine
                EndLine = lastLine
                Lines = mainFunctionLines
            }
        ]

[<Test>]
let ``Categorize two functions``() =
    let mainFunctionLines1 =
        mainFunction
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine1, lastLine1 = mainFunctionLines1 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    let mainFunctionLines2 =
        mainFunction
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine2, lastLine2 = mainFunctionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (Seq.append mainFunctionLines1 mainFunctionLines2)
        [
            {
                Category = FuncDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = mainFunctionLines1
            }
            {
                Category = FuncDefinition
                Index = 1
                File = "example.texton"
                StartLine = firstLine2
                EndLine = lastLine2
                Lines = mainFunctionLines2
            }
        ]

let varDefinition =
    [
        "@var $City"
        "  \"Which city are you writing about?\""
        "  {"
        "    \"London\" [$Country = \"U.K.\"]"
        "    \"Berlin\" [$Country = \"Germany\" && %Gender = \"Male\"]"
        "    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]"
        "    *"
        "  }"
    ]

[<Test>]
let ``Categorize a single variable definition``() =
    let varDefinitionLines =
        varDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine, lastLine = varDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        varDefinitionLines
        [
            {
                Category = VarDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine
                EndLine = lastLine
                Lines = varDefinitionLines
            }
        ]

[<Test>]
let ``Categorize two variable definitions``() =
    let varDefinitionLines1 =
        varDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine1, lastLine1 = varDefinitionLines1 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    let varDefinitionLines2 =
        varDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine2, lastLine2 = varDefinitionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (Seq.append varDefinitionLines1 varDefinitionLines2)
        [
            {
                Category = VarDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = varDefinitionLines1
            }
            {
                Category = VarDefinition
                Index = 1
                File = "example.texton"
                StartLine = firstLine2
                EndLine = lastLine2
                Lines = varDefinitionLines2
            }
        ]

let attDefinition =
    [
        "@att %Gender"
        "  {"
        "    \"Male\""
        "    \"Female\""
        "  }"
    ]

[<Test>]
let ``Categorize a single attribute definition``() =
    let attDefinitionLines =
        attDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine, lastLine = attDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        attDefinitionLines
        [
            {
                Category = AttDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine
                EndLine = lastLine
                Lines = attDefinitionLines
            }
        ]

[<Test>]
let ``Categorize two attribute definitions``() =
    let attDefinitionLines1 =
        attDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine1, lastLine1 = attDefinitionLines1 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    let attDefinitionLines2 =
        attDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = Line(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let firstLine2, lastLine2 = attDefinitionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (Seq.append attDefinitionLines1 attDefinitionLines2)
        [
            {
                Category = AttDefinition
                Index = 0
                File = "example.texton"
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = attDefinitionLines1
            }
            {
                Category = AttDefinition
                Index = 1
                File = "example.texton"
                StartLine = firstLine2
                EndLine = lastLine2
                Lines = attDefinitionLines2
            }
        ]