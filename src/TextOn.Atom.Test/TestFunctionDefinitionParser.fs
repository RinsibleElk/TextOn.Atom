module TextOn.Atom.Test.TestFunctionDefinitionParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

let att n = ParsedAttributeOrVariable.ParsedAttributeName n

[<Test>]
let ``Test sentence with condition``() =
    let funcTokens =
        {   Category = CategorizedFuncDefinition
            Index = 0
            File = "main.texton"
            StartLine = 1
            EndLine = 3
            Tokens =
                [
                    {   LineNumber = 1
                        Tokens =
                            [
                                {   TokenStartLocation = 1
                                    TokenEndLocation = 5
                                    Token = Func }
                                {   TokenStartLocation = 7
                                    TokenEndLocation = 13
                                    Token = FunctionName "main" }
                                {   TokenStartLocation = 12
                                    TokenEndLocation = 12
                                    Token = OpenCurly }
                            ] }
                    {   LineNumber = 2
                        Tokens =
                            [
                                {   TokenStartLocation = 3
                                    TokenEndLocation = 8
                                    Token = RawText "Hello " }
                                {   TokenStartLocation = 9
                                    TokenEndLocation = 9
                                    Token = OpenCurly }
                                {   TokenStartLocation = 10
                                    TokenEndLocation = 14
                                    Token = RawText "world" }
                                {   TokenStartLocation = 15
                                    TokenEndLocation = 15
                                    Token = ChoiceSeparator }
                                {   TokenStartLocation = 16
                                    TokenEndLocation = 20
                                    Token = RawText "earth" }
                                {   TokenStartLocation = 21
                                    TokenEndLocation = 21
                                    Token = CloseCurly }
                                {   TokenStartLocation = 22
                                    TokenEndLocation = 23
                                    Token = RawText ", " }
                                {   TokenStartLocation = 24
                                    TokenEndLocation = 24
                                    Token = OpenCurly }
                                {   TokenStartLocation = 25
                                    TokenEndLocation = 28
                                    Token = RawText "how " }
                                {   TokenStartLocation = 29
                                    TokenEndLocation = 29
                                    Token = OpenCurly }
                                {   TokenStartLocation = 30
                                    TokenEndLocation = 34
                                    Token = RawText "is it" }
                                {   TokenStartLocation = 35
                                    TokenEndLocation = 35
                                    Token = ChoiceSeparator }
                                {   TokenStartLocation = 36
                                    TokenEndLocation = 45
                                    Token = RawText "are things" }
                                {   TokenStartLocation = 46
                                    TokenEndLocation = 46
                                    Token = CloseCurly }
                                {   TokenStartLocation = 47;
                                    TokenEndLocation = 52;
                                    Token = RawText " going";};
                                {   TokenStartLocation = 53;
                                    TokenEndLocation = 53;
                                    Token = ChoiceSeparator;};
                                {   TokenStartLocation = 54;
                                    TokenEndLocation = 61;
                                    Token = RawText "are you ";}
                                {   TokenStartLocation = 62;
                                    TokenEndLocation = 62;
                                    Token = OpenCurly;};
                                {   TokenStartLocation = 63;
                                    TokenEndLocation = 64;
                                    Token = RawText "ok";}
                                {   TokenStartLocation = 65;
                                    TokenEndLocation = 65;
                                    Token = ChoiceSeparator;};
                                {   TokenStartLocation = 66;
                                    TokenEndLocation = 77;
                                    Token = RawText "feeling well";}
                                {   TokenStartLocation = 78;
                                    TokenEndLocation = 78;
                                    Token = CloseCurly;};
                                {   TokenStartLocation = 79;
                                    TokenEndLocation = 84;
                                    Token = RawText " today";}
                                {   TokenStartLocation = 85;
                                    TokenEndLocation = 85;
                                    Token = ChoiceSeparator;};
                                {   TokenStartLocation = 86;
                                    TokenEndLocation = 100;
                                    Token = RawText "what's going on";}
                                {   TokenStartLocation = 101;
                                    TokenEndLocation = 101;
                                    Token = CloseCurly;};
                                {   TokenStartLocation = 102;
                                    TokenEndLocation = 102;
                                    Token = RawText "?";}
                                {   TokenStartLocation = 104;
                                    TokenEndLocation = 104;
                                    Token = OpenBrace }
                                {   TokenStartLocation = 105
                                    TokenEndLocation = 118
                                    Token = AttributeName "SomeAttribute" }
                                {   TokenStartLocation = 120
                                    TokenEndLocation = 120
                                    Token = Equals }
                                {   TokenStartLocation = 122
                                    TokenEndLocation = 133
                                    Token = QuotedString "Some value" }
                                {   TokenStartLocation = 134
                                    TokenEndLocation = 134
                                    Token = CloseBrace }
                            ] }
                    {   LineNumber = 3;
                        Tokens =
                            [
                                {   TokenStartLocation = 1
                                    TokenEndLocation = 1
                                    Token = CloseCurly }
                            ] }
                ] }
    let parsedFunc =
        funcTokens
        |> FunctionDefinitionParser.parseFunctionDefinition
    let expected = {
        StartLine = 1
        EndLine = 3
        Index = 0
        HasErrors = false
        Name = "main"
        Dependencies = [|ParsedAttributeName "SomeAttribute"|]
        Tree =
            ParsedSeq
                [|
                    (   ParsedSentence
                            (   2,
                                ParsedSimpleSeq
                                    [|
                                        ParsedStringValue "Hello "
                                        ParsedSimpleChoice [|ParsedStringValue "world"; ParsedStringValue "earth"|]
                                        ParsedStringValue ", "
                                        ParsedSimpleChoice
                                            [|
                                                ParsedSimpleSeq
                                                    [|
                                                        ParsedStringValue "how "
                                                        ParsedSimpleChoice [|ParsedStringValue "is it";ParsedStringValue "are things"|]
                                                        ParsedStringValue " going"
                                                    |]
                                                ParsedSimpleSeq
                                                    [|
                                                        ParsedStringValue "are you "
                                                        ParsedSimpleChoice [|ParsedStringValue "ok";ParsedStringValue "feeling well"|]
                                                        ParsedStringValue " today"
                                                    |]
                                                ParsedStringValue "what's going on"
                                            |]
                                        ParsedStringValue "?"
                                    |]),
                        ParsedAreEqual (2, 105, 118, (att "SomeAttribute"),"Some value"))
                |] }
    test <@ parsedFunc = expected @>
