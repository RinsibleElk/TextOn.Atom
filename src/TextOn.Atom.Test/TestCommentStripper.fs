module TextOn.Atom.Test.TestCommentStripper

open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

let test s e =
    let r = CommentStripper.stripComments s |> Seq.toList
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

let exampleFileName = "example.texton"

let example =
    [
        (false, "// Here's a function I can call from within the main.", None)
        (true, "@func @guyStuff()", None)
        (true, "{", None)
        (true, "  @choice {", None)
        (true, "    Blah.", None)
        (true, "    Whatever. // Blah blah blah.", Some "    Whatever.")
        (true, "  }", None)
        (true, "}", None)
        (false, "", None)
        (false, "    ", None)
        (false, "  // Every full TextOn script must have a main function.   ", None)
        (true, "@func @main()", None)
        (true, "{", None)
        (true, "  @seq {", None)
        (false, "    // I can include comments at this level too. Note this poses a restriction that if you do want to start your line // we may have to invent an escape.", None)
        (true, "    You are a bloke.", None)
        (true, "    @guyStuff()", None)
        (true, "  }", None)
        (true, "}", None)
        (false, "", None)
    ]

let alwaysFailFileResolver _ _ = None

[<Test>]
let ``CommentStripper with lines``() =
    let expected =
        example
        |> List.scan
            (fun (ln, _) (doInclude,line,replacedLine) ->
                (ln + 1,
                    Some (doInclude,
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = PreprocessorLine (replacedLine |> defaultArg <| line) })))
            (1, None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
        |> List.filter fst
        |> List.map snd
    let input =
        example
        |> List.map (fun (_,line,_) -> line)
        |> Preprocessor.preprocess alwaysFailFileResolver exampleFileName None
    test input expected

[<Test>]
let ``CommentStripper with error``() =
    let input =
        [
            PreprocessorError {
                StartLocation = 1
                EndLocation = 15
                ErrorText = "Some error" }
        ]
        |> List.scan
            (fun (ln,_) line ->
                (ln + 1,
                    Some
                        {
                            TopLevelFileLineNumber = ln
                            CurrentFileLineNumber = ln
                            CurrentFile = exampleFileName
                            Contents = line }))
            (1, None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    test input input
