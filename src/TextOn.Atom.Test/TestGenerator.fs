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
    Variables = [] }

let exampleFileName = "example.texton"
let exampleLineNumber = 1
let makeNoVariablesTemplate node = {
    Attributes = [||]
    Variables = [||]
    Definition = Seq [| (node, True) |] }

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
                    Value = textValue }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test choice``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Choice
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |]
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test sentence break``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |]
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1 }
                {   InputFile = null
                    InputLineNumber = Int32.MinValue
                    Value = "  " }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test paragraph break``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), True)
                (ParagraphBreak(exampleFileName, exampleLineNumber), True)
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |]
    let template = makeNoVariablesTemplate node
    let output = Generator.generate noVariablesInput template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1 }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = "\r\n\r\n" }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>

let makeSingleAttributeTemplate node = {
    Attributes =
        [|
            {
                Name = "Gender"
                Index = 0
                File = exampleFileName
                StartLine = exampleLineNumber
                EndLine = exampleLineNumber
                Values =
                    [|
                        { Value = "Male"; Condition = True }
                        { Value = "Female"; Condition = True }
                    |]
            }
        |]
    Variables = [||]
    Definition = Seq [| (node, True) |] }

let makeSingleAttributeInput gender = {
    RandomSeed = SpecificValue(42)
    Config = {  NumSpacesBetweenSentences = 2
                NumBlankLinesBetweenParagraphs = 1
                LineEnding = CRLF }
    Attributes = [{Name = "Gender";Value = gender}]
    Variables = [] }

[<Test>]
let ``Test successful condition``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), AreEqual(0, "Male"))
                (ParagraphBreak(exampleFileName, exampleLineNumber), AreEqual(0, "Male"))
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |]
    let template = makeSingleAttributeTemplate node
    let output = Generator.generate (makeSingleAttributeInput "Male") template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text1 }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = "\r\n\r\n" }
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>

[<Test>]
let ``Test failed condition``() =
    let text1 = "Hello world."
    let text2 = "Hi earth."
    let node1 = SimpleText text1
    let node2 = SimpleText text2
    let node =
        Seq
            [|
                (Sentence(exampleFileName, exampleLineNumber, node1), AreEqual(0, "Female"))
                (ParagraphBreak(exampleFileName, exampleLineNumber), AreEqual(0, "Female"))
                (Sentence(exampleFileName, exampleLineNumber, node2), True)
            |]
    let template = makeSingleAttributeTemplate node
    let output = Generator.generate (makeSingleAttributeInput "Male") template |> expectSuccess
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>

