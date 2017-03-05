module TextOn.Atom.Test.TestConditionTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

[<Test>]
let ``Valid condition``() =
    let line = "    Hello world. [%Blah = \"Value\"]"
    let expected =
        [
            {TokenStartLocation = 17; TokenEndLocation = 17; Token = OpenBrace }
            {TokenStartLocation = 18; TokenEndLocation = 22; Token = AttributeName "Blah" }
            {TokenStartLocation = 24; TokenEndLocation = 24; Token = Equals }
            {TokenStartLocation = 26; TokenEndLocation = 32; Token = QuotedString "Value" }
            {TokenStartLocation = 33; TokenEndLocation = 33; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 (line.Length - 1) line = expected @>

[<Test>]
let ``Valid condition with a comment after``() =
    let line = "    Hello world. [%Blah = \"Value\"] // We only want this value if condition holds..."
    let expected =
        [
            {TokenStartLocation = 17; TokenEndLocation = 17; Token = OpenBrace }
            {TokenStartLocation = 18; TokenEndLocation = 22; Token = AttributeName "Blah" }
            {TokenStartLocation = 24; TokenEndLocation = 24; Token = Equals }
            {TokenStartLocation = 26; TokenEndLocation = 32; Token = QuotedString "Value" }
            {TokenStartLocation = 33; TokenEndLocation = 33; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 (line.Length - 1) line = expected @>

[<Test>]
let ``Valid condition with a quote in a string value``() =
    let line = "    Hello world. [%Blah = \"\\\" something \\\"\"]   "
    let expected =
        [
            {TokenStartLocation = 17; TokenEndLocation = 17; Token = OpenBrace }
            {TokenStartLocation = 18; TokenEndLocation = 22; Token = AttributeName "Blah" }
            {TokenStartLocation = 24; TokenEndLocation = 24; Token = Equals }
            {TokenStartLocation = 26; TokenEndLocation = 42; Token = QuotedString "\" something \"" }
            {TokenStartLocation = 43; TokenEndLocation = 43; Token = CloseBrace }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 (line.Length - 1) line = expected @>

[<Test>]
let ``Valid condition with a comment in a string``() =
    let line = "    Hello world. [%Blah = \"// Something\"] "
    let expected =
        [
            {TokenStartLocation = 17; TokenEndLocation = 17; Token = OpenBrace }
            {TokenStartLocation = 18; TokenEndLocation = 22; Token = AttributeName "Blah" }
            {TokenStartLocation = 24; TokenEndLocation = 24; Token = Equals }
            {TokenStartLocation = 26; TokenEndLocation = 39; Token = QuotedString "// Something" }
            {TokenStartLocation = 40; TokenEndLocation = 40; Token = CloseBrace }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 (line.Length - 1) line = expected @>
