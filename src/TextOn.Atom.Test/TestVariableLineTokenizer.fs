module TextOn.Atom.Test.TestVariableLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

let test source expected received =
    Seq.zip3
        source
        expected
        received
    |> Seq.iter
        (fun (s, e, r) ->
            let eLen = e |> List.length
            let rLen = r |> List.length
            let eTrun = e |> List.truncate rLen
            let rTrun = r |> List.truncate eLen
            let firstIncorrect = List.zip eTrun rTrun |> List.tryFind (fun (e,r) -> e <> r) |> Option.map (fun (e,r) -> sprintf "%A\n\n<>\n\n%A" e r)
            if eLen > rLen then
                failwithf "Too short for %s - got %d tokens, expected %d tokens, first missing token %A, first incorrect token %A" s rLen eLen (e |> List.skip (rLen) |> List.head) firstIncorrect
            else if eLen < rLen then
                failwithf "Too long for %s - got %d tokens, expected %d tokens, first extra token %A, first incorrect token %A" s rLen eLen (r |> List.skip (eLen) |> List.head) firstIncorrect
            else if firstIncorrect.IsSome then
                failwithf "Incorrect token for %s %s" s firstIncorrect.Value)

[<Test>]
let ``Correctly formatted variable definition``() =
    let lines =
        [
            ("@var $City", [{StartIndex = 1;EndIndex = 4;Token = Var};{StartIndex = 6;EndIndex = 10;Token = VariableName "City"}])
            ("  \"Which city are you writing about?\"", [{StartIndex = 3;EndIndex = 37;Token = QuotedString "Which city are you writing about?"}])
            ("  {", [{StartIndex = 3;EndIndex = 3;Token = OpenCurly}])
            ("    \"London\" [$Country = \"U.K.\"]", [{StartIndex = 5;EndIndex = 12;Token = QuotedString "London"};{StartIndex = 14;EndIndex = 14;Token = OpenBrace};{StartIndex = 15;EndIndex = 22;Token = VariableName("Country")};{StartIndex = 24;EndIndex = 24;Token = Equals};{StartIndex = 26;EndIndex = 31;Token = QuotedString "U.K."};{StartIndex = 32;EndIndex = 32;Token = CloseBrace}])
            ("    \"Berlin\" [$Country = \"Germany\" || %Gender = \"Male\"]", [{StartIndex = 5;EndIndex = 12;Token = QuotedString "Berlin"};{StartIndex = 14;EndIndex = 14;Token = OpenBrace};{StartIndex = 15;EndIndex = 22;Token = VariableName("Country")};{StartIndex = 24;EndIndex = 24;Token = Equals};{StartIndex = 26;EndIndex = 34;Token = QuotedString "Germany"};{StartIndex = 36;EndIndex = 37;Token = Or};{StartIndex = 39;EndIndex = 45;Token=AttributeName("Gender")};{StartIndex = 47;EndIndex = 47;Token = Equals};{StartIndex = 49;EndIndex = 54;Token = QuotedString "Male"};{StartIndex = 55;EndIndex = 55;Token = CloseBrace}])
            ("    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]", [{StartIndex = 5;EndIndex = 11;Token = QuotedString "Paris"};{StartIndex = 13;EndIndex = 13;Token = OpenBrace};{StartIndex = 14;EndIndex = 21;Token = VariableName("Country")};{StartIndex = 23;EndIndex = 24;Token = NotEquals};{StartIndex = 26;EndIndex = 34;Token = QuotedString "Germany"};{StartIndex = 36;EndIndex = 37;Token = And};{StartIndex = 39;EndIndex = 46;Token=VariableName("Country")};{StartIndex = 48;EndIndex = 49;Token = NotEquals};{StartIndex = 51;EndIndex = 56;Token = QuotedString "U.K."};{StartIndex = 57;EndIndex = 57;Token = CloseBrace}])
            ("    *", [{StartIndex = 5;EndIndex = 5;Token = Star}])
            ("  }", [{StartIndex = 3;EndIndex = 3;Token = CloseCurly}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableLineTokenizer.tokenizeLine >> List.ofSeq)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens

[<Test>]
let ``Correctly formatted variable definition with funky characters``() =
    let lines =
        [
            ("@var $Cîty", [{StartIndex = 1;EndIndex = 4;Token = Var};{StartIndex = 6;EndIndex = 10;Token = VariableName "Cîty"}])
            ("  \"Which city áre you writing about?\"", [{StartIndex = 3;EndIndex = 37;Token = QuotedString "Which city áre you writing about?"}])
            ("  {", [{StartIndex = 3;EndIndex = 3;Token = OpenCurly}])
            ("    \"London\" [$Cöuntry = \"U.K.\"]", [{StartIndex = 5;EndIndex = 12;Token = QuotedString "London"};{StartIndex = 14;EndIndex = 14;Token = OpenBrace};{StartIndex = 15;EndIndex = 22;Token = VariableName("Cöuntry")};{StartIndex = 24;EndIndex = 24;Token = Equals};{StartIndex = 26;EndIndex = 31;Token = QuotedString "U.K."};{StartIndex = 32;EndIndex = 32;Token = CloseBrace}])
            ("    \"Berlin\" [$Country = \"Germany\" || %Gønder = \"Male\"]", [{StartIndex = 5;EndIndex = 12;Token = QuotedString "Berlin"};{StartIndex = 14;EndIndex = 14;Token = OpenBrace};{StartIndex = 15;EndIndex = 22;Token = VariableName("Country")};{StartIndex = 24;EndIndex = 24;Token = Equals};{StartIndex = 26;EndIndex = 34;Token = QuotedString "Germany"};{StartIndex = 36;EndIndex = 37;Token = Or};{StartIndex = 39;EndIndex = 45;Token=AttributeName("Gønder")};{StartIndex = 47;EndIndex = 47;Token = Equals};{StartIndex = 49;EndIndex = 54;Token = QuotedString "Male"};{StartIndex = 55;EndIndex = 55;Token = CloseBrace}])
            ("    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]", [{StartIndex = 5;EndIndex = 11;Token = QuotedString "Paris"};{StartIndex = 13;EndIndex = 13;Token = OpenBrace};{StartIndex = 14;EndIndex = 21;Token = VariableName("Country")};{StartIndex = 23;EndIndex = 24;Token = NotEquals};{StartIndex = 26;EndIndex = 34;Token = QuotedString "Germany"};{StartIndex = 36;EndIndex = 37;Token = And};{StartIndex = 39;EndIndex = 46;Token=VariableName("Country")};{StartIndex = 48;EndIndex = 49;Token = NotEquals};{StartIndex = 51;EndIndex = 56;Token = QuotedString "U.K."};{StartIndex = 57;EndIndex = 57;Token = CloseBrace}])
            ("    *", [{StartIndex = 5;EndIndex = 5;Token = Star}])
            ("  }", [{StartIndex = 3;EndIndex = 3;Token = CloseCurly}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableLineTokenizer.tokenizeLine >> List.ofSeq)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens

[<Test>]
let ``Bad variable lines``() =
    let lines =
        [
            ("    \"London", [{StartIndex = 1;EndIndex = 11;Token = Unrecognised}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableLineTokenizer.tokenizeLine >> List.ofSeq)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens
