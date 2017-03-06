module TextOn.Atom.Test.TestConditionParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

let att n = ParsedAttributeOrVariable.ParsedAttributeName n
let var n = ParsedAttributeOrVariable.ParsedVariableName n
let exampleFileName = "example.texton"

[<Test>]
let ``Test simple equals``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 18 18 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition = ParsedAreEqual(2, 2, 8, att "Gender", "Male") }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test simple not equals``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 11 NotEquals
            makeToken 13 18 (QuotedString "Male")
            makeToken 19 19 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition = ParsedAreNotEqual(2, 2, 8, att "Gender", "Male") }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test bracketed equals``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 2 OpenBracket
            makeToken 3 9 (AttributeName "Gender")
            makeToken 11 11 Equals
            makeToken 13 18 (QuotedString "Male")
            makeToken 19 19 CloseBracket
            makeToken 20 20 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition = ParsedAreEqual(2, 3, 9, att "Gender", "Male") }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test bracketed not equals``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 2 OpenBracket
            makeToken 3 9 (AttributeName "Gender")
            makeToken 11 12 NotEquals
            makeToken 14 19 (QuotedString "Male")
            makeToken 20 20 CloseBracket
            makeToken 21 21 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition = ParsedAreNotEqual(2, 3, 9, att "Gender", "Male") }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test single or``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 19 20 Or
            makeToken 22 28 (AttributeName "Gender")
            makeToken 30 30 Equals
            makeToken 32 39 (QuotedString "Female")
            makeToken 40 40 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedOr(
                ParsedAreEqual(2, 2, 8, att "Gender", "Male"),
                ParsedAreEqual(2, 22, 28, att "Gender", "Female")) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test multiple ors``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 19 20 Or
            makeToken 22 28 (AttributeName "Gender")
            makeToken 30 30 Equals
            makeToken 32 39 (QuotedString "Female")
            makeToken 41 42 Or
            makeToken 44 50 (AttributeName "Gender")
            makeToken 52 52 Equals
            makeToken 54 72 (QuotedString "Attack helicopter")
            makeToken 74 75 Or
            makeToken 77 83 (AttributeName "Gender")
            makeToken 85 85 Equals
            makeToken 87 93 (QuotedString "Other")
            makeToken 94 94 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedOr(
                ParsedAreEqual(2, 2, 8, att "Gender", "Male"),
                ParsedOr(
                    ParsedAreEqual(2, 22, 28, att "Gender", "Female"),
                    ParsedOr(
                        ParsedAreEqual(2, 44, 50, att "Gender", "Attack helicopter"),
                        ParsedAreEqual(2, 77, 83, att "Gender", "Other")))) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test single and``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 19 20 And
            makeToken 22 28 (AttributeName "Gender")
            makeToken 30 30 Equals
            makeToken 32 39 (QuotedString "Female")
            makeToken 40 40 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedAnd(
                ParsedAreEqual(2, 2, 8, att "Gender", "Male"),
                ParsedAreEqual(2, 22, 28, att "Gender", "Female")) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test multiple ands``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 19 20 And
            makeToken 22 28 (AttributeName "Gender")
            makeToken 30 30 Equals
            makeToken 32 39 (QuotedString "Female")
            makeToken 41 42 And
            makeToken 44 50 (AttributeName "Gender")
            makeToken 52 52 Equals
            makeToken 54 72 (QuotedString "Attack helicopter")
            makeToken 74 75 And
            makeToken 77 83 (AttributeName "Gender")
            makeToken 85 85 Equals
            makeToken 87 93 (QuotedString "Other")
            makeToken 94 94 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedAnd(
                ParsedAreEqual(2, 2, 8, att "Gender", "Male"),
                ParsedAnd(
                    ParsedAreEqual(2, 22, 28, att "Gender", "Female"),
                    ParsedAnd(
                        ParsedAreEqual(2, 44, 50, att "Gender", "Attack helicopter"),
                        ParsedAreEqual(2, 77, 83, att "Gender", "Other")))) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test and/or precedence``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 8 (AttributeName "Gender")
            makeToken 10 10 Equals
            makeToken 12 17 (QuotedString "Male")
            makeToken 19 20 And
            makeToken 22 28 (AttributeName "Gender")
            makeToken 30 30 Equals
            makeToken 32 39 (QuotedString "Female")
            makeToken 41 42 Or
            makeToken 44 50 (AttributeName "Gender")
            makeToken 52 52 Equals
            makeToken 54 72 (QuotedString "Attack helicopter")
            makeToken 74 75 And
            makeToken 77 83 (AttributeName "Gender")
            makeToken 85 85 Equals
            makeToken 87 93 (QuotedString "Other")
            makeToken 94 94 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedOr(
                ParsedAnd(
                    ParsedAreEqual (2, 2, 8, att "Gender","Male"),
                    ParsedAreEqual (2, 22, 28, att "Gender","Female")),
                ParsedAnd(
                    ParsedAreEqual (2, 44, 50, att "Gender","Attack helicopter"),
                    ParsedAreEqual (2, 77, 83, att "Gender","Other"))) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

[<Test>]
let ``Test brackets``() =
    let makeToken s e t = { TokenStartLocation = s ; TokenEndLocation = e ; Token = t }
    // [(%Gender = "Male" || %Gender = "Female") || %Gender = "Attack helicopter" && %Gender = "Other"]
    let tokens =
        [
            makeToken 1 1 OpenBrace
            makeToken 2 2 OpenBracket
            makeToken 3 9 (AttributeName "Gender")
            makeToken 11 11 Equals
            makeToken 13 18 (QuotedString "Male")
            makeToken 20 21 Or
            makeToken 23 29 (AttributeName "Gender")
            makeToken 31 31 Equals
            makeToken 33 40 (QuotedString "Female")
            makeToken 41 41 CloseBracket
            makeToken 43 44 Or
            makeToken 46 52 (AttributeName "Gender")
            makeToken 54 54 Equals
            makeToken 56 74 (QuotedString "Attack helicopter")
            makeToken 76 77 And
            makeToken 79 85 (AttributeName "Gender")
            makeToken 87 87 Equals
            makeToken 89 95 (QuotedString "Other")
            makeToken 96 96 CloseBrace
        ]
    let expected = {
        HasErrors = false
        Dependencies = [|att "Gender"|]
        Condition =
            ParsedOr(
                ParsedOr(
                    ParsedAreEqual (2, 3, 9, att "Gender","Male"),
                    ParsedAreEqual (2, 23, 29, att "Gender","Female")),
                ParsedAnd(
                    ParsedAreEqual (2, 46, 52, att "Gender","Attack helicopter"),
                    ParsedAreEqual (2, 79, 85, att "Gender","Other"))) }
    test <@ ConditionParser.parseCondition exampleFileName 2 false tokens = expected @>

