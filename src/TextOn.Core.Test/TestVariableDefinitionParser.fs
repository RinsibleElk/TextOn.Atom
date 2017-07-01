module TextOn.Core.Test.TestVariableDefinitionParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Core

let exampleFileName = "example.texton"
let private makeTokenSet tokens =
    {
        Category = Category.CategorizedVarDefinition
        File = exampleFileName
        StartLine = 1
        EndLine = tokens |> List.length
        Tokens =
            tokens
            |> List.mapi
                (fun l t ->
                    {   LineNumber = l + 1
                        Tokens = t |> List.map (fun (s,e,t) -> { TokenStartLocation = s ; TokenEndLocation = e ; Token  = t }) })
    }

[<Test>]
let ``Free variable with no suggestions``() =
    let text = "Which country are you writing about?"
    let tokens =
        [
            (Var, 4)
            (Free, 5)
            (VariableName "Country", 8)
            (Equals, 1)
            (QuotedString text, text.Length + 2)
        ]
        |> List.scan (fun (s, _) (t, l) -> (s + l + 1, (Some (s, s + l - 1, t)))) (1, None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
        |> List.singleton
        |> makeTokenSet
    let expected = {
        StartLine = 1
        EndLine = 1
        Name = "Country"
        Text = text
        SupportsFreeValue = true
        Dependencies = [||]
        Result = [||] }
    test <@ VariableDefinitionParser.parseVariableDefinition tokens = ([||], Some expected) @>

[<Test>]
let ``Two line free variable with no suggestions``() =
    let text = "Which country are you writing about?"
    let tokens1 =
        [
            (Var, 4)
            (Free, 5)
            (VariableName "Country", 8)
            (Equals, 1)
        ]
        |> List.scan (fun (s, _) (t, l) -> (s + l + 1, (Some (s, s + l - 1, t)))) (1, None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let tokens2 =
        [
            (QuotedString text, text.Length + 2)
        ]
        |> List.scan (fun (s, _) (t, l) -> (s + l + 1, (Some (s, s + l - 1, t)))) (3, None)
        |> List.skip 1
        |> List.map (snd >> Option.get)
    let expected = {
        StartLine = 1
        EndLine = 2
        Name = "Country"
        Text = text
        SupportsFreeValue = true
        Dependencies = [||]
        Result = [||] }
    test <@ VariableDefinitionParser.parseVariableDefinition (makeTokenSet [tokens1;tokens2]) = ([||], Some expected) @>
