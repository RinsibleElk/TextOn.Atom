module TextOn.Atom.Test.TestPreprocessor

open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open System.IO
open TextOn.Atom

let alwaysFailFileResolver _ _ = None

let exampleFileName = "example.texton"
let exampleDirectory = @"D:\Example"

let test f s e =
    let r = Preprocessor.preprocess f exampleFileName exampleDirectory s |> Seq.toList
    let e = e |> Seq.toList
    let firstFailingLine =
        List.zip
            (r |> List.truncate (e |> List.length))
            (e |> List.truncate (r |> List.length))
        |> List.tryFind (fun (r,e) -> r <> e)
        |> Option.map (fun (r,e) -> sprintf "\n%A\n\n!=\n\n%A" e r)
    if (r |> List.length) <> (e |> List.length) then
        failwithf "The lists had different lengths - expected %d, result was %d, first failing line was: %A" (e |> List.length) (r |> List.length) firstFailingLine
    else if firstFailingLine |> Option.isSome then
        failwithf "The lists didn't match - first failing line was: %s" firstFailingLine.Value

let gender =
    [
        "@att %Gender ="
        "  ["
        "    \"Male\","
        "    \"Female\""
        "  ]"
        "@att %Pronoun ="
        "  ["
        "    \"He\" [%Gender = \"Male\"],"
        "    \"She\" [%Gender = \"Female\"]"
        "  ]"
    ]
let example =
    [
        "// Here's a function I can call from within the main."
        "@func @guyStuff()"
        "{"
        "  @choice {"
        "    Blah."
        "    Whatever."
        "  }"
        "}"
        ""
        "// Every full TextOn script must have a main function."
        "@func @main()"
        "{"
        "  @seq {"
        "    You are a bloke."
        "    @guyStuff()"
        "  }"
        "}"
    ]

[<Test>]
let ``Preprocessor with nothing to do``() =
    test alwaysFailFileResolver [] []

[<Test>]
let ``Preprocessor with no includes``() =
    let expected =
        example
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = ln
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    test alwaysFailFileResolver example expected

[<Test>]
let ``Preprocessor with single successful include``() =
    let source =
        seq {
            yield "#include \"gender.texton\""
            yield! example }
        |> Seq.toList
    let fileResolver : PreprocessorFileResolver = (fun _ _ -> Some ("gender.texton", exampleDirectory, gender))
    let expectedGender =
        gender
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = 1
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, "gender.texton")
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
        |> Seq.toList
    let expected =
        source
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = ln
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 2
        |> Seq.map (snd >> Option.get)
        |> Seq.append expectedGender
        |> Seq.toList
    test fileResolver source expected

[<Test>]
let ``Preprocessor with double successful include``() =
    let source =
        seq {
            yield "#include \"gender.texton\""
            yield "#include \"gender.texton\""
            yield! example }
        |> Seq.toList
    let fileResolver : PreprocessorFileResolver = (fun _ _ -> Some ("gender.texton", exampleDirectory, gender))
    let expectedGender =
        gender
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = 1
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, "gender.texton")
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 1
        |> Seq.map (snd >> Option.get)
    let expected =
        source
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = ln
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 3
        |> Seq.map (snd >> Option.get)
        |> fun l ->
            seq {
                yield! expectedGender
                yield! l }
        |> Seq.toList
    test fileResolver source expected

[<Test>]
let ``Preprocessor with single failed include``() =
    let source =
        seq {
            yield "#include \"gender.texton\""
            yield! example }
        |> Seq.toList
    let expectedError = {
        TopLevelFileLineNumber = 1
        CurrentFileLineNumber = 1
        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
        Contents = PreprocessorError {
            StartLocation = 10
            EndLocation = 24
            ErrorText = "Unable to resolve file: gender.texton" } }
    let expected =
        source
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = ln
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 2
        |> Seq.map (snd >> Option.get)
        |> fun l ->
            seq {
                yield expectedError
                yield! l }
        |> Seq.toList
    test alwaysFailFileResolver source expected

[<Test>]
let ``Preprocessor with unrecognised directive``() =
    let source =
        seq {
            yield "#whatever \"Something\"  15"
            yield! example }
        |> Seq.toList
    let expectedError = {
        TopLevelFileLineNumber = 1
        CurrentFileLineNumber = 1
        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
        Contents = PreprocessorError {
            StartLocation = 1
            EndLocation = 25
            ErrorText = "Not a valid #include directive: #whatever \"Something\"  15" } }
    let expected =
        source
        |> Seq.scan
            (fun (ln,output) line ->
                (ln + 1),
                    Some {
                        TopLevelFileLineNumber = ln
                        CurrentFileLineNumber = ln
                        CurrentFile = Path.Combine(exampleDirectory, exampleFileName)
                        Contents = PreprocessorLine line
                    })
            (1,None)
        |> Seq.skip 2
        |> Seq.map (snd >> Option.get)
        |> fun l ->
            seq {
                yield expectedError
                yield! l }
        |> Seq.toList
    test alwaysFailFileResolver source expected

