module TextOn.Atom.Test.TestGenerator

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

let noVariablesInput = {
    RandomSeed = SpecificValue(42)
    Config = {  NumSpacesBetweenSentences = 2
                NumBlankLinesBetweenParagraphs = 1
                LineEnding = CRLF }
    Attributes = []
    Variables = []
    Function = "main" }

let exampleFileName = "example.texton"
let exampleLineNumber = 1
let makeNoVariablesTemplate node = {
    Attributes = [||]
    Variables = [||]
    Functions =
        [|
            {
                Name = "main"
                Index = 1
                File = exampleFileName
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                AttributeDependencies = [||]
                VariableDependencies = [||]
                Tree = Seq("", 0, [| (node, True) |])
            }
        |] }

let expectSuccess result =
    match result with
    | GeneratorError e -> failwithf "Expected success but got error: %s" e
    | GeneratorSuccess s -> s

[<Test>]
let ``Test simple text``() =
    let textValue = "Hello world."
    let simpleNode = SimpleText textValue
    let template = makeNoVariablesTemplate (Sentence(exampleFileName, exampleLineNumber, simpleNode))
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = textValue
                    IsPb = false }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test choice``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Choice(
            "",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2
                    IsPb = false }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test sentence break``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq(
            "",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1
                    IsPb = false }
                {   InputFile = null
                    InputLineNumber = Int32.MinValue
                    Value = "  "
                    IsPb = false }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2
                    IsPb = false }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test paragraph break``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq(
            "",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (ParagraphBreak(exampleFileName, exampleLineNumber), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1
                    IsPb = false }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = "\r\n\r\n"
                    IsPb = true }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2
                    IsPb = false }
            |] }
    test <@ expected = output @>

let makeSingleAttributeTemplate node = {
    Attributes =
        [|
            {
                Name = "Gender"
                Text = "What is the gender of the target audience?"
                Index = 0
                File = exampleFileName
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                AttributeDependencies= [||]
                Values =
                    [|
                        { Value = "Male"; Condition = True }
                        { Value = "Female"; Condition = True }
                    |]
            }
        |]
    Variables = [||]
    Functions =
        [|
            {
                Name = "main"
                Index = 1
                File = exampleFileName
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                AttributeDependencies = [|0|]
                VariableDependencies = [||]
                Tree = Seq("", 0, [| (node, True) |])
            }
        |] }

let makeSingleAttributeInput gender = {
    RandomSeed = SpecificValue(42)
    Config = {  NumSpacesBetweenSentences = 2
                NumBlankLinesBetweenParagraphs = 1
                LineEnding = CRLF }
    Attributes = [{Name = "Gender";Value = gender}]
    Variables = []
    Function = "main" }

[<Test>]
let ``Test successful condition``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq("",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), AreEqual(0, "Male"))
                (ParagraphBreak(exampleFileName, exampleLineNumber), AreEqual(0, "Male"))
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template = makeSingleAttributeTemplate node
    let output = Generator.generate (makeSingleAttributeInput "Male") template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1
                    IsPb = false }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = "\r\n\r\n"
                    IsPb = true }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2
                    IsPb = false }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test failed condition``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq(
            "",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), AreEqual(0, "Female"))
                (ParagraphBreak(exampleFileName, exampleLineNumber), AreEqual(0, "Female"))
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template = makeSingleAttributeTemplate node
    let output = Generator.generate (makeSingleAttributeInput "Male") template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2
                    IsPb = false }
            |] }
    test <@ expected = output @>

let makeSingleVariableTemplate node = {
    Attributes = [||]
    Variables =
        [|
            {
                Name = "Country"
                Index = 0
                File = exampleFileName
                Text = ""
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                PermitsFreeValue = true
                AttributeDependencies = [||]
                VariableDependencies = [||]
                Values =
                    [|
                        { Value = "U.K."; Condition = VarTrue }
                        { Value = "Germany"; Condition = VarTrue }
                    |]
            }
        |]
    Functions =
        [|
            {
                Name = "main"
                Index = 1
                File = exampleFileName
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                AttributeDependencies = [||]
                VariableDependencies = [|0|]
                Tree = Seq("", 0, [| (node, True) |])
            }
        |] }

let makeSingleVariableInput country = {
    RandomSeed = SpecificValue(42)
    Config = {  NumSpacesBetweenSentences = 2
                NumBlankLinesBetweenParagraphs = 1
                LineEnding = CRLF }
    Attributes = []
    Variables = [{Name = "Country";Value = country}]
    Function = "main" }

[<Test>]
let ``Test variable replacement``() =
    let node = Sentence(exampleFileName, exampleLineNumber, VariableValue 0)
    let template = makeSingleVariableTemplate node
    let countryText = "U.K."
    let output = Generator.generate (makeSingleVariableInput countryText) template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = countryText
                    IsPb = false }
            |] }
    test <@ expected = output @>

let makeNoVariablesInputWithSeed seed = {
    RandomSeed = seed
    Config = {  NumSpacesBetweenSentences = 2
                NumBlankLinesBetweenParagraphs = 1
                LineEnding = CRLF }
    Attributes = []
    Variables = []
    Function = "main" }

[<Test>]
let ``Test using same seed gets same value``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Choice(
            "",
            0,
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |])
    let template1 = makeNoVariablesTemplate node
    let expected = Generator.generate (makeNoVariablesInputWithSeed NoSeed) template1 |> expectSuccess
    let template2 = makeNoVariablesTemplate node
    let output = Generator.generate (makeNoVariablesInputWithSeed (SpecificValue expected.LastSeed)) template2 |> expectSuccess
    test <@ expected = output @>
