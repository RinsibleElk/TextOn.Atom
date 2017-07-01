module TextOn.Core.Test.TestOutsideLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core

let private runTest expected line =
    let expectedPlus4 = expected |> List.map (fun (token, s, e) -> { Token = token ; TokenStartLocation = s + 4 ; TokenEndLocation = e + 4 })
    let expected = expected |> List.map (fun (token, s, e) -> { Token = token ; TokenStartLocation = s ; TokenEndLocation = e })
    let lineWithSpaceBefore = "    " + line
    let lineWithSpaceAfter = line + "    "
    let lineWithCommentAfter = line + " // Some comment"
    test <@ OutsideLineTokenizer.tokenizeLine line = expected @>
    test <@ OutsideLineTokenizer.tokenizeLine lineWithSpaceBefore = expectedPlus4 @>
    test <@ OutsideLineTokenizer.tokenizeLine lineWithSpaceAfter = expected @>
    test <@ OutsideLineTokenizer.tokenizeLine lineWithCommentAfter = expected @>

[<Test>]
let ``Legacy include line``() =
    let line = "#include \"hello.texton\""
    let expected =
        [
            (Include, 1, 8)
            (QuotedString "hello.texton", 10, 23)
        ]
    runTest expected line

[<Test>]
let ``Import line``() =
    let line = "@import \"hello.texton\""
    let expected =
        [
            (Import, 1, 7)
            (QuotedString "hello.texton", 9, 22)
        ]
    runTest expected line

[<Test>]
let ``Function start line no open curly``() =
    let line = "@func @hello"
    let expected =
        [
            (Func, 1, 5)
            (FunctionName "hello", 7, 12)
        ]
    runTest expected line

[<Test>]
let ``Private function start line no open curly``() =
    let line = "@func @private @hello"
    let expected =
        [
            (Func, 1, 5)
            (Private, 7, 14)
            (FunctionName "hello", 16, 21)
        ]
    runTest expected line

[<Test>]
let ``Function start line``() =
    let line = "@func @hello {"
    let expected =
        [
            (Func, 1, 5)
            (FunctionName "hello", 7, 12)
            (OpenCurly, 14, 14)
        ]
    runTest expected line

[<Test>]
let ``Private function start line``() =
    let line = "@func @private @hello {"
    let expected =
        [
            (Func, 1, 5)
            (Private, 7, 14)
            (FunctionName "hello", 16, 21)
            (OpenCurly, 23, 23)
        ]
    runTest expected line

[<Test>]
let ``Blank line``() =
    let line = ""
    let expected =
        [
        ]
    runTest expected line

[<Test>]
let ``Comment line``() =
    let line = "// Some comment"
    let expected =
        [
        ]
    runTest expected line

[<Test>]
let ``Attribute start line``() =
    let line = "@att %SomeAttribute ="
    let expected =
        [
            (Att, 1, 4)
            (AttributeName "SomeAttribute", 6, 19)
            (Equals, 21, 21)
        ]
    runTest expected line

[<Test>]
let ``Attribute start line with description``() =
    let line = "@att %SomeAttribute = \"Here is a description\""
    let expected =
        [
            (Att, 1, 4)
            (AttributeName "SomeAttribute", 6, 19)
            (Equals, 21, 21)
            (QuotedString "Here is a description", 23, 45)
        ]
    runTest expected line

[<Test>]
let ``Attribute start line with description and curly``() =
    let line = "@att %SomeAttribute = \"Here is a description\" {"
    let expected =
        [
            (Att, 1, 4)
            (AttributeName "SomeAttribute", 6, 19)
            (Equals, 21, 21)
            (QuotedString "Here is a description", 23, 45)
            (OpenCurly, 47, 47)
        ]
    runTest expected line

[<Test>]
let ``Variable start line``() =
    let line = "@var $SomeVariable ="
    let expected =
        [
            (Var, 1, 4)
            (VariableName "SomeVariable", 6, 18)
            (Equals, 20, 20)
        ]
    runTest expected line

[<Test>]
let ``Variable start line with description``() =
    let line = "@var $SomeVariable = \"Here is a description\""
    let expected =
        [
            (Var, 1, 4)
            (VariableName "SomeVariable", 6, 18)
            (Equals, 20, 20)
            (QuotedString "Here is a description", 22, 44)
        ]
    runTest expected line

[<Test>]
let ``Variable start line with description and curly``() =
    let line = "@var $SomeVariable = \"Here is a description\" {"
    let expected =
        [
            (Var, 1, 4)
            (VariableName "SomeVariable", 6, 18)
            (Equals, 20, 20)
            (QuotedString "Here is a description", 22, 44)
            (OpenCurly, 46, 46)
        ]
    runTest expected line

[<Test>]
let ``Free variable start line``() =
    let line = "@var @free $SomeVariable ="
    let expected =
        [
            (Var, 1, 4)
            (Free, 6, 10)
            (VariableName "SomeVariable", 12, 24)
            (Equals, 26, 26)
        ]
    runTest expected line

[<Test>]
let ``Free variable start line with description``() =
    let line = "@var @free $SomeVariable = \"Here is a description\""
    let expected =
        [
            (Var, 1, 4)
            (Free, 6, 10)
            (VariableName "SomeVariable", 12, 24)
            (Equals, 26, 26)
            (QuotedString "Here is a description", 28, 50)
        ]
    runTest expected line

[<Test>]
let ``Free variable start line with description and curly``() =
    let line = "@var @free $SomeVariable = \"Here is a description\" {"
    let expected =
        [
            (Var, 1, 4)
            (Free, 6, 10)
            (VariableName "SomeVariable", 12, 24)
            (Equals, 26, 26)
            (QuotedString "Here is a description", 28, 50)
            (OpenCurly, 52, 52)
        ]
    runTest expected line

