module TextOn.Core.Test.TestConditionTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core

[<Test>]
let ``Valid condition``() =
    let line = "    Hello world. [%Blah = \"Value\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 34; TokenEndLocation = 34; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with a comment after``() =
    let line = "    Hello world. [%Blah = \"Value\"] // We only want this value if condition holds..."
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 34; TokenEndLocation = 34; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with a quote in a string value``() =
    let line = "    Hello world. [%Blah = \"\\\" something \\\"\"]   "
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 43; Token = QuotedString "\" something \"" }
            {TokenStartLocation = 44; TokenEndLocation = 44; Token = CloseBrace }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with a comment in a string``() =
    let line = "    Hello world. [%Blah = \"// Something\"] "
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 40; Token = QuotedString "// Something" }
            {TokenStartLocation = 41; TokenEndLocation = 41; Token = CloseBrace }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

