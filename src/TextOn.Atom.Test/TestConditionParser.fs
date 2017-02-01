module TextOn.Atom.Test.TestConditionParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

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
        Condition = ParsedAreEqual("Gender", "Male") }
    test <@ ConditionParser.parseCondition tokens = expected @>

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
        Condition = ParsedAreNotEqual("Gender", "Male") }
    test <@ ConditionParser.parseCondition tokens = expected @>

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
        Condition = ParsedAreEqual("Gender", "Male") }
    test <@ ConditionParser.parseCondition tokens = expected @>

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
        Condition = ParsedAreNotEqual("Gender", "Male") }
    test <@ ConditionParser.parseCondition tokens = expected @>

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
        Condition = ParsedOr(ParsedAreEqual("Gender", "Male"), ParsedAreEqual("Gender", "Female")) }
    test <@ ConditionParser.parseCondition tokens = expected @>

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
        Condition =
            ParsedOr(
                ParsedAreEqual("Gender", "Male"),
                ParsedOr(
                    ParsedAreEqual("Gender", "Female"),
                    ParsedOr(
                        ParsedAreEqual("Gender", "Attack helicopter"),
                        ParsedAreEqual("Gender", "Other")))) }
    test <@ ConditionParser.parseCondition tokens = expected @>

