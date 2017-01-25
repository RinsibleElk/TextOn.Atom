module TextOn.Atom.Test.TestDefinitionLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

[<Test>]
let ``Valid full function``() =
    [
        ("@func @main", [{StartIndex = 1;EndIndex = 5;Token = Func}; {StartIndex = 7;EndIndex = 11;Token = FunctionName "main"}])
        ("{", [{StartIndex = 1;EndIndex = 1;Token = OpenCurly}])
        ("  @seq {", [{StartIndex = 3;EndIndex = 6;Token = Sequential}; {StartIndex = 8;EndIndex = 8;Token = OpenCurly}])
        ("    You are a bloke. [%Gender = \"Male\"]", [{StartIndex = 5;EndIndex = 20;Token = RawText "You are a bloke."}; {StartIndex = 22;EndIndex = 22;Token = OpenBrace}; {StartIndex = 23;EndIndex = 29;Token = AttributeName "Gender"}; {StartIndex = 31;EndIndex = 31;Token = Equals}; {StartIndex = 33;EndIndex = 38;Token = QuotedString "Male"}; {StartIndex = 39;EndIndex = 39;Token = CloseBrace}])
        ("    You live in {$City|a city in $Country}.", [{StartIndex = 5;EndIndex = 16;Token = RawText "You live in "}; {StartIndex = 17;EndIndex = 17;Token = OpenCurly}; {StartIndex = 18;EndIndex = 22;Token = VariableName "City"}; {StartIndex = 23;EndIndex = 23;Token = ChoiceSeparator}; {StartIndex = 24;EndIndex = 33;Token = RawText "a city in "}; {StartIndex = 34;EndIndex = 41;Token = VariableName "Country"}; {StartIndex = 42;EndIndex = 42;Token = CloseCurly}; {StartIndex = 43;EndIndex = 43;Token = RawText "."}])
        ("    We are in $City which is in $Country.", [{StartIndex = 5;EndIndex = 14;Token = RawText "We are in "}; {StartIndex = 15;EndIndex = 19;Token = VariableName "City"}; {StartIndex = 20;EndIndex = 32;Token = RawText " which is in "}; {StartIndex = 33;EndIndex = 40;Token = VariableName "Country"}; {StartIndex = 41;EndIndex = 41;Token = RawText "."}])
        ("    @break", [{StartIndex = 5;EndIndex = 10;Token = Break}])
        ("    @guyStuff [%Gender = \"Male\"]", [{StartIndex = 5;EndIndex = 13;Token = FunctionName "guyStuff"}; {StartIndex = 15;EndIndex = 15;Token = OpenBrace}; {StartIndex = 16;EndIndex = 22;Token = AttributeName "Gender"}; {StartIndex = 24;EndIndex = 24;Token = Equals}; {StartIndex = 26;EndIndex = 31;Token = QuotedString "Male"}; {StartIndex = 32;EndIndex = 32;Token = CloseBrace}])
        ("  }", [{StartIndex = 3;EndIndex = 3;Token = CloseCurly}])
        ("}", [{StartIndex = 1;EndIndex = 1;Token = CloseCurly}])
    ]
    |> Seq.iter
        (fun (line,expected) ->
            let result = DefinitionLineTokenizer.tokenizeLine line |> Seq.toList
            if (result <> expected) then
                failwithf "Didn't get expected result %A\n\n<>\n\n%A" expected result)

[<Test>]
let ``Invalid - only an escape character``() =
    let result = DefinitionLineTokenizer.tokenizeLine "\\" |> Seq.toList
    let expected = [{StartIndex=1;EndIndex=1;Token=InvalidUnrecognised "\\" }]
    if result <> expected then
        failwithf "Didn't get expected result %A\n\n<>\n\n%A" expected result

