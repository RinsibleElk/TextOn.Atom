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

let private runTest intersperseWithBlankLines input =
    let input =
        if (not intersperseWithBlankLines) then
            input
        else
            input
            |> List.map (fun (a,l) -> (a, l |> List.collect (fun (a,b) -> [(a,b);("",[])]) |> fun l -> l |> List.take (l.Length - 1)))
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
        |> List.map (fst >> Option.get >> (fun e -> { e with Tokens = e.Tokens |> List.filter (fun t -> t.Tokens.Length > 0) }))
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

let private importLines =
    [
        (CategorizedImport,
            [
                ("@import \"somefile.texton\"", [(Import, 1, 7);(QuotedString "somefile.texton", 9, 25)])
            ])
    ]

let private unrecognisedLines =
    [
        (CategorizationError "Unrecognised starting token",
            [
                ("Blah", [(InvalidUnrecognised "Blah", 1, 4)])
            ])
    ]

let private sustainedErrorLines =
    [
        (CategorizationError "Unrecognised starting token",
            [
                ("Blah", [(InvalidUnrecognised "Blah", 1, 4)])
                ("Something", [(InvalidUnrecognised "Something", 1, 9)])
                ("Something or other", [(InvalidUnrecognised "Something or other", 1, 18)])
            ])
    ]

[<Test>]
let ``Valid full function``() =
    runTest false funcLines

[<Test>]
let ``Valid free variable``() =
    runTest false varLines

[<Test>]
let ``Valid attribute``() =
    runTest false attLines

[<Test>]
let ``Valid attribute, variable, then function``() =
    runTest false (attLines@varLines@funcLines)

[<Test>]
let ``Invalid unrecognised token``() =
    runTest false unrecognisedLines

[<Test>]
let ``Unfinished attribute``() =
    let (_, l) = attLines.[0]
    runTest false [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished variable``() =
    let (_, l) = varLines.[0]
    runTest false [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function``() =
    let (_, l) = funcLines.[0]
    runTest false [(CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function followed by attribute``() =
    let (_, l) = funcLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by function``() =
    let (_, l) = funcLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by variable``() =
    let (_, l) = funcLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by import``() =
    let (_, l) = funcLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by attribute``() =
    let (_, l) = varLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by function``() =
    let (_, l) = varLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by variable``() =
    let (_, l) = varLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by import``() =
    let (_, l) = varLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by attribute``() =
    let (_, l) = attLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by function``() =
    let (_, l) = attLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by variable``() =
    let (_, l) = attLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by import``() =
    let (_, l) = attLines.[0]
    runTest false
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Sustained error followed by attribute``() =
    runTest false (sustainedErrorLines @ attLines)

[<Test>]
let ``Sustained error followed by function``() =
    runTest false (sustainedErrorLines @ funcLines)

[<Test>]
let ``Sustained error followed by variable``() =
    runTest false (sustainedErrorLines @ varLines)

[<Test>]
let ``Sustained error followed by import``() =
    runTest false (sustainedErrorLines @ importLines)

[<Test>]
let ``Valid full function interspersed``() =
    runTest true funcLines

[<Test>]
let ``Valid free variable interspersed``() =
    runTest true varLines

[<Test>]
let ``Valid attribute interspersed``() =
    runTest true attLines

[<Test>]
let ``Valid attribute, variable, then function interspersed``() =
    runTest true (attLines@varLines@funcLines)

[<Test>]
let ``Invalid unrecognised token interspersed``() =
    runTest true unrecognisedLines

[<Test>]
let ``Unfinished attribute interspersed``() =
    let (_, l) = attLines.[0]
    runTest true [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished variable interspersed``() =
    let (_, l) = varLines.[0]
    runTest true [(CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function interspersed``() =
    let (_, l) = funcLines.[0]
    runTest true [(CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))]

[<Test>]
let ``Unfinished function followed by attribute interspersed``() =
    let (_, l) = funcLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by function interspersed``() =
    let (_, l) = funcLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by variable interspersed``() =
    let (_, l) = funcLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished function followed by import interspersed``() =
    let (_, l) = funcLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete function definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by attribute interspersed``() =
    let (_, l) = varLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by function interspersed``() =
    let (_, l) = varLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by variable interspersed``() =
    let (_, l) = varLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished variable followed by import interspersed``() =
    let (_, l) = varLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by attribute interspersed``() =
    let (_, l) = attLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            attLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by function interspersed``() =
    let (_, l) = attLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            funcLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by variable interspersed``() =
    let (_, l) = attLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            varLines.[0]
        ]

[<Test>]
let ``Unfinished attribute followed by import interspersed``() =
    let (_, l) = attLines.[0]
    runTest true
        [
            (CategorizationError "Incomplete variable/attribute definition", l |> List.take (l.Length - 1))
            importLines.[0]
        ]

[<Test>]
let ``Sustained error followed by attribute interspersed``() =
    runTest true (sustainedErrorLines @ attLines)

[<Test>]
let ``Sustained error followed by function interspersed``() =
    runTest true (sustainedErrorLines @ funcLines)

[<Test>]
let ``Sustained error followed by variable interspersed``() =
    runTest true (sustainedErrorLines @ varLines)

[<Test>]
let ``Sustained error followed by import interspersed``() =
    runTest true (sustainedErrorLines @ importLines)

