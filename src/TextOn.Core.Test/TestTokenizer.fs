module TextOn.Core.Test.Tokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core

let exampleFileName = @"D:\NodeJs\TextOn\example.texton"

let private convertInputs lineNumber l =
    l
    |> List.scan
        (fun (_, line) tokens ->
            tokens
            |> List.map (fun (token, s, e) -> { Token = token ; TokenStartLocation = s ; TokenEndLocation = e })
            |> fun tokens -> { LineNumber = line ; Tokens = tokens }
            |> Some
            |> fun o -> (o, line + 1))
        (None, lineNumber)
    |> List.skip 1
    |> List.map (fst >> Option.get)

let private runTest input =
    let lines = input |> List.collect (snd >> List.map fst)
    let expected =
        input
        |> List.scan
            (fun (_, lineNumber) (category,inputs) ->
                let numLines = inputs |> List.length
                let expected =
                    {
                        Category = category
                        File = exampleFileName
                        StartLine = lineNumber
                        EndLine = lineNumber + numLines - 1
                        Tokens = convertInputs lineNumber (inputs |> List.map snd)
                    }
                (Some expected, lineNumber + numLines))
            (None, 1)
        |> List.skip 1
        |> List.map (fst >> Option.get)
    let result = Tokenizer.tokenize exampleFileName lines
    test <@ result = expected @>

[<Test>]
let ``Valid full function``() =
    let lines =
        [
            (CategorizedFuncDefinition,
                [
                    ("@func @main", [(Func, 1, 5); (FunctionName "main", 7, 11)])
                    ("{", [(OpenCurly, 1, 1)])
                    ("  @seq {", [(Sequential, 3, 6);(OpenCurly, 8, 8)])
                    ("    You are a bloke. [%Gender = \"Male\"]", [(RawText "You are a bloke.", 5, 20); (OpenBrace, 22, 22); (AttributeName "Gender", 23, 29); (Equals, 31, 31); (QuotedString "Male", 33, 38); (CloseBrace, 39, 39)])
                    ("    You live in {$City|a city in $Country}.", [(RawText "You live in ", 5, 16); (OpenCurly, 17, 17); (VariableName "City", 18, 22); (ChoiceSeparator, 23, 23); (RawText "a city in ", 24, 33); (VariableName "Country", 34, 41); (CloseCurly, 42, 42); (RawText ".", 43, 43)])
                    ("    We are in $City which is in $Country.", [(RawText "We are in ", 5, 14); (VariableName "City", 15, 19); (RawText " which is in ", 20, 32); (VariableName "Country", 33, 40); (RawText ".", 41, 41)])
                    ("    @break", [(Break, 5, 10)])
                    ("    @guyStuff [%Gender = \"Male\"]", [(FunctionName "guyStuff", 5, 13); (OpenBrace, 15, 15); (AttributeName "Gender", 16, 22); (Equals, 24, 24); (QuotedString "Male", 26, 31); (CloseBrace, 32, 32)])
                    ("  }", [(CloseCurly, 3, 3)])
                    ("}", [(CloseCurly, 1, 1)])
                ])
        ]
    runTest lines
