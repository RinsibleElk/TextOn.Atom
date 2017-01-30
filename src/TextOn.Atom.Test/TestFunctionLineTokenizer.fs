module TextOn.Atom.Test.TestFunctionLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

[<Test>]
let ``Valid full function``() =
    [
        ("@func @main", [{TokenStartLocation = 1;TokenEndLocation = 5;Token = Func}; {TokenStartLocation = 7;TokenEndLocation = 11;Token = FunctionName "main"}])
        ("{", [{TokenStartLocation = 1;TokenEndLocation = 1;Token = OpenCurly}])
        ("  @seq {", [{TokenStartLocation = 3;TokenEndLocation = 6;Token = Sequential}; {TokenStartLocation = 8;TokenEndLocation = 8;Token = OpenCurly}])
        ("    You are a bloke. [%Gender = \"Male\"]", [{TokenStartLocation = 5;TokenEndLocation = 20;Token = RawText "You are a bloke."}; {TokenStartLocation = 22;TokenEndLocation = 22;Token = OpenBrace}; {TokenStartLocation = 23;TokenEndLocation = 29;Token = AttributeName "Gender"}; {TokenStartLocation = 31;TokenEndLocation = 31;Token = Equals}; {TokenStartLocation = 33;TokenEndLocation = 38;Token = QuotedString "Male"}; {TokenStartLocation = 39;TokenEndLocation = 39;Token = CloseBrace}])
        ("    You live in {$City|a city in $Country}.", [{TokenStartLocation = 5;TokenEndLocation = 16;Token = RawText "You live in "}; {TokenStartLocation = 17;TokenEndLocation = 17;Token = OpenCurly}; {TokenStartLocation = 18;TokenEndLocation = 22;Token = VariableName "City"}; {TokenStartLocation = 23;TokenEndLocation = 23;Token = ChoiceSeparator}; {TokenStartLocation = 24;TokenEndLocation = 33;Token = RawText "a city in "}; {TokenStartLocation = 34;TokenEndLocation = 41;Token = VariableName "Country"}; {TokenStartLocation = 42;TokenEndLocation = 42;Token = CloseCurly}; {TokenStartLocation = 43;TokenEndLocation = 43;Token = RawText "."}])
        ("    We are in $City which is in $Country.", [{TokenStartLocation = 5;TokenEndLocation = 14;Token = RawText "We are in "}; {TokenStartLocation = 15;TokenEndLocation = 19;Token = VariableName "City"}; {TokenStartLocation = 20;TokenEndLocation = 32;Token = RawText " which is in "}; {TokenStartLocation = 33;TokenEndLocation = 40;Token = VariableName "Country"}; {TokenStartLocation = 41;TokenEndLocation = 41;Token = RawText "."}])
        ("    @break", [{TokenStartLocation = 5;TokenEndLocation = 10;Token = Break}])
        ("    @guyStuff [%Gender = \"Male\"]", [{TokenStartLocation = 5;TokenEndLocation = 13;Token = FunctionName "guyStuff"}; {TokenStartLocation = 15;TokenEndLocation = 15;Token = OpenBrace}; {TokenStartLocation = 16;TokenEndLocation = 22;Token = AttributeName "Gender"}; {TokenStartLocation = 24;TokenEndLocation = 24;Token = Equals}; {TokenStartLocation = 26;TokenEndLocation = 31;Token = QuotedString "Male"}; {TokenStartLocation = 32;TokenEndLocation = 32;Token = CloseBrace}])
        ("  }", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = CloseCurly}])
        ("}", [{TokenStartLocation = 1;TokenEndLocation = 1;Token = CloseCurly}])
    ]
    |> Seq.iter
        (fun (line,expected) ->
            let result = FunctionLineTokenizer.tokenizeLine line |> Seq.toList
            if (result <> expected) then
                failwithf "Didn't get expected result %A\n\n<>\n\n%A" expected result)

[<Test>]
let ``Invalid - only an escape character``() =
    let result = FunctionLineTokenizer.tokenizeLine "\\" |> Seq.toList
    let expected = [{TokenStartLocation=1;TokenEndLocation=1;Token=InvalidUnrecognised "\\" }]
    if result <> expected then
        failwithf "Didn't get expected result %A\n\n<>\n\n%A" expected result

