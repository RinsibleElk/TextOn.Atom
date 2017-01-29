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

[<Test>]
let ``Test simple text``() =
    let textValue = "Hello world."
    let simpleNode = SimpleText textValue
    let template = makeNoVariablesTemplate (Sentence(exampleFileName, exampleLineNumber, simpleNode))
    let output = Generator.generate noVariablesInput template
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
    let output = Generator.generate noVariablesInput template
    let expected = {
        LastSeed = 42
        Text =
            [|
                {   InputFile = exampleFileName
                    InputLineNumber = exampleLineNumber
                    Value = text2 }
            |] }
    test <@ expected = output @>
