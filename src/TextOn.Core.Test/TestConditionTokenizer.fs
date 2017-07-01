module TextOn.Core.Test.TestConditionTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core.Tokenizing

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

[<Test>]
let ``Invalid condition ends with <``() =
    let line = "    Hello world. [%Blah <"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = InvalidUnrecognised "<" }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid condition < not followed by >``() =
    let line = "    Hello world. [%Blah <= \"Something\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 39; Token = InvalidUnrecognised "<= \"Something\"]" }
        ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid condition ends with &``() =
    let line = "    Hello world. [%Blah = \"Value\" &"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = InvalidUnrecognised "&" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid condition & not followed by &``() =
    let line = "    Hello world. [%Blah = \"Value\" &| %Blahe2 = \"SomeText\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 58; Token = InvalidUnrecognised "&| %Blahe2 = \"SomeText\"]" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid condition ends with |``() =
    let line = "    Hello world. [%Blah = \"Value\" |"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = InvalidUnrecognised "|" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid condition | not followed by |``() =
    let line = "    Hello world. [%Blah = \"Value\" |& %Blahe2 = \"SomeText\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 58; Token = InvalidUnrecognised "|& %Blahe2 = \"SomeText\"]" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with an or``() =
    let line = "    Hello world. [%Blah = \"Value\" || %Blahe2 = \"SomeText\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 36; Token = Or }
            {TokenStartLocation = 38; TokenEndLocation = 44; Token = AttributeName "Blahe2" }
            {TokenStartLocation = 46; TokenEndLocation = 46; Token = Equals }
            {TokenStartLocation = 48; TokenEndLocation = 57; Token = QuotedString "SomeText" }
            {TokenStartLocation = 58; TokenEndLocation = 58; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with an and``() =
    let line = "    Hello world. [%Blah = \"Value\" && %Blahe2 = \"SomeText\"]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 23; Token = AttributeName "Blah" }
            {TokenStartLocation = 25; TokenEndLocation = 25; Token = Equals }
            {TokenStartLocation = 27; TokenEndLocation = 33; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 36; Token = And }
            {TokenStartLocation = 38; TokenEndLocation = 44; Token = AttributeName "Blahe2" }
            {TokenStartLocation = 46; TokenEndLocation = 46; Token = Equals }
            {TokenStartLocation = 48; TokenEndLocation = 57; Token = QuotedString "SomeText" }
            {TokenStartLocation = 58; TokenEndLocation = 58; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Valid condition with brackets``() =
    let line = "    Hello world. [(%Blah = \"Value\") && (%Blahe2 = \"SomeText\")]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = AttributeName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 26; Token = Equals }
            {TokenStartLocation = 28; TokenEndLocation = 34; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = CloseBracket }
            {TokenStartLocation = 37; TokenEndLocation = 38; Token = And }
            {TokenStartLocation = 40; TokenEndLocation = 40; Token = OpenBracket }
            {TokenStartLocation = 41; TokenEndLocation = 47; Token = AttributeName "Blahe2" }
            {TokenStartLocation = 49; TokenEndLocation = 49; Token = Equals }
            {TokenStartLocation = 51; TokenEndLocation = 60; Token = QuotedString "SomeText" }
            {TokenStartLocation = 61; TokenEndLocation = 61; Token = CloseBracket }
            {TokenStartLocation = 62; TokenEndLocation = 62; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``No attribute name``() =
    let line = "    Hello world. [(%Blah = \"Value\") && (% = \"SomeText\")]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = AttributeName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 26; Token = Equals }
            {TokenStartLocation = 28; TokenEndLocation = 34; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = CloseBracket }
            {TokenStartLocation = 37; TokenEndLocation = 38; Token = And }
            {TokenStartLocation = 40; TokenEndLocation = 40; Token = OpenBracket }
            {TokenStartLocation = 41; TokenEndLocation = 41; Token = InvalidUnrecognised "%" }
            {TokenStartLocation = 43; TokenEndLocation = 43; Token = Equals }
            {TokenStartLocation = 45; TokenEndLocation = 54; Token = QuotedString "SomeText" }
            {TokenStartLocation = 55; TokenEndLocation = 55; Token = CloseBracket }
            {TokenStartLocation = 56; TokenEndLocation = 56; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``No variable name``() =
    let line = "    \"ello world\" [($Blah = \"Value\") && ($ = \"SomeText\")]"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = VariableName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 26; Token = Equals }
            {TokenStartLocation = 28; TokenEndLocation = 34; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = CloseBracket }
            {TokenStartLocation = 37; TokenEndLocation = 38; Token = And }
            {TokenStartLocation = 40; TokenEndLocation = 40; Token = OpenBracket }
            {TokenStartLocation = 41; TokenEndLocation = 41; Token = InvalidUnrecognised "$" }
            {TokenStartLocation = 43; TokenEndLocation = 43; Token = Equals }
            {TokenStartLocation = 45; TokenEndLocation = 54; Token = QuotedString "SomeText" }
            {TokenStartLocation = 55; TokenEndLocation = 55; Token = CloseBracket }
            {TokenStartLocation = 56; TokenEndLocation = 56; Token = CloseBrace }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid comment - one slash``() =
    let line = "    \"ello world\" [($Blah = \"Value\")] / I forgot the second slash"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = VariableName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 26; Token = Equals }
            {TokenStartLocation = 28; TokenEndLocation = 34; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = CloseBracket }
            {TokenStartLocation = 36; TokenEndLocation = 36; Token = CloseBrace }
            {TokenStartLocation = 38; TokenEndLocation = 64; Token = InvalidUnrecognised "/ I forgot the second slash" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Invalid comment - ends with a slash``() =
    let line = "    \"ello world\" [($Blah = \"Value\")] /"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = VariableName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 26; Token = Equals }
            {TokenStartLocation = 28; TokenEndLocation = 34; Token = QuotedString "Value" }
            {TokenStartLocation = 35; TokenEndLocation = 35; Token = CloseBracket }
            {TokenStartLocation = 36; TokenEndLocation = 36; Token = CloseBrace }
            {TokenStartLocation = 38; TokenEndLocation = 38; Token = InvalidUnrecognised "/" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>

[<Test>]
let ``Bad character``() =
    let line = "    \"ello world\" [($Blah ... = \"Value\")] /"
    let lastIndex = line.Length - 1
    let expected =
        [
            {TokenStartLocation = 18; TokenEndLocation = 18; Token = OpenBrace }
            {TokenStartLocation = 19; TokenEndLocation = 19; Token = OpenBracket }
            {TokenStartLocation = 20; TokenEndLocation = 24; Token = VariableName "Blah" }
            {TokenStartLocation = 26; TokenEndLocation = 42; Token = InvalidUnrecognised "... = \"Value\")] /" }
    ]
    test <@ ConditionTokenizer.tokenizeCondition 17 lastIndex line = expected @>
