module TextOn.Atom.Test.TestLineCategorizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

let test s (e:CategorizedLines list) =
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
let exampleFileName2 = "example2.texton"

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
                    PreprocessorError
                        {
                            StartLocation = 1
                            EndLocation = 5
                            ErrorText = "Some error text"
                        }
            }
        ]
        [
            {
                Category = CategorizedPreprocessorError
                Index = 0
                File = exampleFileName
                StartLine = 1
                EndLine = 1
                Lines =
                    Seq.singleton
                        {
                            TopLevelFileLineNumber = 1
                            CurrentFileLineNumber = 1
                            CurrentFile = exampleFileName
                            Contents =
                                PreprocessorError {
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
                    PreprocessorWarning
                        {
                            StartLocation = 1
                            EndLocation = 5
                            WarningText = "Some warning text"
                        }
            }
        ]
        [
            {
                Category = CategorizedPreprocessorWarning
                Index = 0
                File = exampleFileName
                StartLine = 1
                EndLine = 1
                Lines =
                    Seq.singleton
                        {
                            TopLevelFileLineNumber = 1
                            CurrentFileLineNumber = 1
                            CurrentFile = exampleFileName
                            Contents =
                                PreprocessorWarning {
                                    StartLocation = 1
                                    EndLocation = 5
                                    WarningText = "Some warning text" }
                    }
            }
        ]

let funcDefinition =
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
let ``Categorize a single function definition``() =
    let funcDefinitionLines =
        funcDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine, lastLine = funcDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        funcDefinitionLines
        [
            {
                Category = CategorizedFuncDefinition
                Index = 0
                File = exampleFileName
                StartLine = firstLine
                EndLine = lastLine
                Lines = funcDefinitionLines
            }
        ]

[<Test>]
let ``Categorize two function definitions``() =
    let funcDefinitionLines1 =
        funcDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine1, lastLine1 = funcDefinitionLines1 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    let funcDefinitionLines2 =
        funcDefinition
        |> Seq.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine2, lastLine2 = funcDefinitionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append funcDefinitionLines1 funcDefinitionLines2)
        [
            {
                Category = CategorizedFuncDefinition
                Index = 0
                File = exampleFileName
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = funcDefinitionLines1
            }
            {
                Category = CategorizedFuncDefinition
                Index = 1
                File = exampleFileName
                StartLine = firstLine2
                EndLine = lastLine2
                Lines = funcDefinitionLines2
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
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine, lastLine = varDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        varDefinitionLines
        [
            {
                Category = CategorizedVarDefinition
                Index = 0
                File = exampleFileName
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
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
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
                            Contents = PreprocessorLine(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine2, lastLine2 = varDefinitionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append varDefinitionLines1 varDefinitionLines2)
        [
            {
                Category = CategorizedVarDefinition
                Index = 0
                File = exampleFileName
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = varDefinitionLines1
            }
            {
                Category = CategorizedVarDefinition
                Index = 1
                File = exampleFileName
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
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine, lastLine = attDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        attDefinitionLines
        [
            {
                Category = CategorizedAttDefinition
                Index = 0
                File = exampleFileName
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
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
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
                            Contents = PreprocessorLine(line) }))
            ((lastLine1 + 1),None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let firstLine2, lastLine2 = attDefinitionLines2 |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append attDefinitionLines1 attDefinitionLines2)
        [
            {
                Category = CategorizedAttDefinition
                Index = 0
                File = exampleFileName
                StartLine = firstLine1
                EndLine = lastLine1
                Lines = attDefinitionLines1
            }
            {
                Category = CategorizedAttDefinition
                Index = 1
                File = exampleFileName
                StartLine = firstLine2
                EndLine = lastLine2
                Lines = attDefinitionLines2
            }
        ]

[<Test>]
let ``Categorize a whole single file``() =
    let parse firstLine x =
        let s =
            x
            |> Seq.scan
                (fun (ln,_) line ->
                    (ln,
                        Some
                            {
                                TopLevelFileLineNumber = ln
                                CurrentFileLineNumber = ln
                                CurrentFile = exampleFileName
                                Contents = PreprocessorLine(line) }))
                (firstLine,None)
            |> Seq.skip 1
            |> Seq.map (snd >> Option.get)
        let firstLine, lastLine = s |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
        s, firstLine, lastLine
    let preprocessedLines =
        [
            (CategorizedAttDefinition, attDefinition)
            (CategorizedVarDefinition, varDefinition)
            (CategorizedFuncDefinition, funcDefinition)
            (CategorizedAttDefinition, attDefinition)
            (CategorizedVarDefinition, varDefinition)
            (CategorizedFuncDefinition, funcDefinition)
        ]
        |> Seq.scan
            (fun (firstLine,_) (category, lines) ->
                let (s, firstLine, lastLine) = parse firstLine lines
                ((lastLine + 1), Some (s, firstLine, lastLine, category)))
            (1, None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let input =
        preprocessedLines
        |> Seq.collect (fun (a,_,_,_) -> a)
        |> Seq.toList
    let expected =
        preprocessedLines
        |> Seq.scan
            (fun (index,_) (a,b,c,d) ->
                ((index + 1),
                    Some
                        {
                            Category = d
                            Index = index
                            File = exampleFileName
                            StartLine = b
                            EndLine = c
                            Lines = a
                        }))
            (0,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> List.ofSeq
    test input expected

[<Test>]
let ``Unrecognised then function definition``() =
    let unrecognisedLines =
        [
            (1, "hello")
            (2, "world")
        ]
        |> List.map
            (fun (ln, line) ->
                {   TopLevelFileLineNumber = ln
                    CurrentFileLineNumber = ln
                    CurrentFile = exampleFileName
                    Contents = PreprocessorLine(line) })
    let funcDefinitionLines =
        funcDefinition
        |> List.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (3,None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let firstLine, lastLine = funcDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append unrecognisedLines funcDefinitionLines)
        [
            {
                Category = CategorizationError
                Index = 0
                File = exampleFileName
                StartLine = 1
                EndLine = 2
                Lines = unrecognisedLines
            }
            {
                Category = CategorizedFuncDefinition
                Index = 1
                File = exampleFileName
                StartLine = firstLine
                EndLine = lastLine
                Lines = funcDefinitionLines
            }
        ]

[<Test>]
let ``Unrecognised then variable definition``() =
    let unrecognisedLines =
        [
            (1, "hello")
            (2, "world")
        ]
        |> List.map
            (fun (ln, line) ->
                {   TopLevelFileLineNumber = ln
                    CurrentFileLineNumber = ln
                    CurrentFile = exampleFileName
                    Contents = PreprocessorLine(line) })
    let varDefinitionLines =
        varDefinition
        |> List.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (3,None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let firstLine, lastLine = varDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append unrecognisedLines varDefinitionLines)
        [
            {
                Category = CategorizationError
                Index = 0
                File = exampleFileName
                StartLine = 1
                EndLine = 2
                Lines = unrecognisedLines
            }
            {
                Category = CategorizedVarDefinition
                Index = 1
                File = exampleFileName
                StartLine = firstLine
                EndLine = lastLine
                Lines = varDefinitionLines
            }
        ]

[<Test>]
let ``Unrecognised then attribute definition``() =
    let unrecognisedLines =
        [
            (1, "hello")
            (2, "world")
        ]
        |> List.map
            (fun (ln, line) ->
                {   TopLevelFileLineNumber = ln
                    CurrentFileLineNumber = ln
                    CurrentFile = exampleFileName
                    Contents = PreprocessorLine(line) })
    let attDefinitionLines =
        attDefinition
        |> List.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (3,None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let firstLine, lastLine = attDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    test
        (List.append unrecognisedLines attDefinitionLines)
        [
            {
                Category = CategorizationError
                Index = 0
                File = exampleFileName
                StartLine = 1
                EndLine = 2
                Lines = unrecognisedLines
            }
            {
                Category = CategorizedAttDefinition
                Index = 1
                File = exampleFileName
                StartLine = firstLine
                EndLine = lastLine
                Lines = attDefinitionLines
            }
        ]

[<Test>]
let ``Unrecognised new file``() =
    let attDefinitionLines =
        attDefinition
        |> List.scan
            (fun (ln,_) line ->
                (ln,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine(line) }))
            (1,None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let firstLine, lastLine = attDefinitionLines |> Seq.fold (fun (mn,mx) l -> (min mn l.CurrentFileLineNumber, max mx l.CurrentFileLineNumber)) (Int32.MaxValue, Int32.MinValue)
    let unrecognisedLines =
        [
            (2, "hello")
            (3, "world")
        ]
        |> List.map
            (fun (ln, line) ->
                {   TopLevelFileLineNumber = ln
                    CurrentFileLineNumber = ln
                    CurrentFile = exampleFileName2
                    Contents = PreprocessorLine(line) })
    test
        (List.append attDefinitionLines unrecognisedLines)
        [
            {
                Category = CategorizedAttDefinition
                Index = 0
                File = exampleFileName
                StartLine = firstLine
                EndLine = lastLine
                Lines = attDefinitionLines
            }
            {
                Category = CategorizationError
                Index = 1
                File = exampleFileName2
                StartLine = 2
                EndLine = 3
                Lines = unrecognisedLines
            }
        ]


