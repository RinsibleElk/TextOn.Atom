module TextOn.Core.Test.TestTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core.Tokenizing

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

let private funcLines =
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

let private varLines =
    [
        (CategorizedVarDefinition,
            [
                ("@var @free $City =", [(Var, 1, 4);(Free, 6, 10);(VariableName "City", 12, 16);(Equals, 18, 18)])
                ("  \"Which city are you writing about?\"", [(QuotedString "Which city are you writing about?", 3, 37)])
                ("  {", [(OpenCurly, 3, 3)])
                ("    \"London\" [$Country = \"U.K.\"] // Comment after condition", [(QuotedString "London", 5, 12);(OpenBrace, 14, 14);(VariableName("Country"), 15, 22);(Equals, 24, 24);(QuotedString "U.K.", 26, 31);(CloseBrace, 32, 32)])
                ("    \"Berlin\" [$Country = \"Germany\" || %Gender = \"Male\"]", [(QuotedString "Berlin", 5, 12);(OpenBrace, 14, 14);(VariableName("Country"), 15, 22);(Equals, 24, 24);(QuotedString "Germany", 26, 34);(Or, 36, 37);(AttributeName "Gender", 39, 45);(Equals, 47, 47);(QuotedString "Male", 49, 54);(CloseBrace, 55, 55)])
                ("    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]", [(QuotedString "Paris", 5, 11);(OpenBrace, 13, 13);(VariableName("Country"), 14, 21);(NotEquals, 23, 24);(QuotedString "Germany", 26, 34);(And, 36, 37);(VariableName("Country"), 39, 46);(NotEquals, 48, 49);(QuotedString "U.K.", 51, 56);(CloseBrace, 57, 57)])
                ("  }", [(CloseCurly, 3, 3)])
            ])
    ]

let private attLines =
    [
        (CategorizedAttDefinition,
            [
                ("@att %Gender = \"What is the gender of your target audience?\"", [(Att, 1, 4);(AttributeName "Gender", 6, 12);(Equals, 14, 14);(QuotedString "What is the gender of your target audience?", 16, 60)])
                ("  {", [(OpenCurly, 3, 3)])
                ("    \"Male\" // Comment after no condition", [(QuotedString "Male", 5, 10)])
                ("    \"Female\"", [(QuotedString "Female", 5, 12)])
                ("  }", [(CloseCurly, 3, 3)])
            ])
    ]

let private unrecognisedLines =
    [
        (CategorizationError "Unrecognised starting token",
            [
                ("Blah", [(InvalidUnrecognised "Blah", 1, 4)])
            ])
    ]

[<Test>]
let ``Valid full function``() =
    runTest funcLines

[<Test>]
let ``Valid free variable``() =
    runTest varLines

[<Test>]
let ``Valid attribute``() =
    runTest attLines

[<Test>]
let ``Valid attribute, variable, then function``() =
    runTest (attLines@varLines@funcLines)

[<Test>]
let ``Invalid unrecognised token``() =
    runTest unrecognisedLines

[<Test>]
let ``Unfinished attribute``() =
    let (_, l) = attLines.[0]
    runTest [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished variable``() =
    let (_, l) = varLines.[0]
    runTest [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function``() =
    let (_, l) = funcLines.[0]
    runTest [(CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function followed by attribute``() =
    let (_, l) = funcLines.[0]
    runTest
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by function``() =
    let (_, l) = funcLines.[0]
    runTest
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by variable``() =
    let (_, l) = funcLines.[0]
    runTest
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by import``() =
    let (_, l) = funcLines.[0]
    runTest
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by attribute``() =
    let (_, l) = varLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by function``() =
    let (_, l) = varLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by variable``() =
    let (_, l) = varLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by import``() =
    let (_, l) = varLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by attribute``() =
    let (_, l) = attLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by function``() =
    let (_, l) = attLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by variable``() =
    let (_, l) = attLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by import``() =
    let (_, l) = attLines.[0]
    runTest
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

