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
            ("@var @free $City =", [{TokenStartLocation = 1;TokenEndLocation = 4;Token = Var};{TokenStartLocation = 6;TokenEndLocation = 10;Token = Free};{TokenStartLocation = 12; TokenEndLocation = 16; Token = VariableName "City"};{TokenStartLocation = 18; TokenEndLocation = 18; Token = Equals}])
            ("  \"Which city are you writing about?\"", [{TokenStartLocation = 3;TokenEndLocation = 37;Token = QuotedString "Which city are you writing about?"}])
            ("  {", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = OpenCurly}])
            ("    \"London\" [$Country = \"U.K.\"]", [{TokenStartLocation = 5;TokenEndLocation = 12;Token = QuotedString "London"};{TokenStartLocation = 14;TokenEndLocation = 14;Token = OpenBrace};{TokenStartLocation = 15;TokenEndLocation = 22;Token = VariableName("Country")};{TokenStartLocation = 24;TokenEndLocation = 24;Token = Equals};{TokenStartLocation = 26;TokenEndLocation = 31;Token = QuotedString "U.K."};{TokenStartLocation = 32;TokenEndLocation = 32;Token = CloseBrace}])
            ("    \"Berlin\" [$Country = \"Germany\" || %Gender = \"Male\"]", [{TokenStartLocation = 5;TokenEndLocation = 12;Token = QuotedString "Berlin"};{TokenStartLocation = 14;TokenEndLocation = 14;Token = OpenBrace};{TokenStartLocation = 15;TokenEndLocation = 22;Token = VariableName("Country")};{TokenStartLocation = 24;TokenEndLocation = 24;Token = Equals};{TokenStartLocation = 26;TokenEndLocation = 34;Token = QuotedString "Germany"};{TokenStartLocation = 36;TokenEndLocation = 37;Token = Or};{TokenStartLocation = 39;TokenEndLocation = 45;Token=AttributeName("Gender")};{TokenStartLocation = 47;TokenEndLocation = 47;Token = Equals};{TokenStartLocation = 49;TokenEndLocation = 54;Token = QuotedString "Male"};{TokenStartLocation = 55;TokenEndLocation = 55;Token = CloseBrace}])
            ("    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]", [{TokenStartLocation = 5;TokenEndLocation = 11;Token = QuotedString "Paris"};{TokenStartLocation = 13;TokenEndLocation = 13;Token = OpenBrace};{TokenStartLocation = 14;TokenEndLocation = 21;Token = VariableName("Country")};{TokenStartLocation = 23;TokenEndLocation = 24;Token = NotEquals};{TokenStartLocation = 26;TokenEndLocation = 34;Token = QuotedString "Germany"};{TokenStartLocation = 36;TokenEndLocation = 37;Token = And};{TokenStartLocation = 39;TokenEndLocation = 46;Token=VariableName("Country")};{TokenStartLocation = 48;TokenEndLocation = 49;Token = NotEquals};{TokenStartLocation = 51;TokenEndLocation = 56;Token = QuotedString "U.K."};{TokenStartLocation = 57;TokenEndLocation = 57;Token = CloseBrace}])
            ("  }", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = CloseCurly}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableOrAttributeLineTokenizer.tokenizeLine >> List.ofSeq)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens

[<Test>]
let ``Correctly formatted variable definition with funky characters``() =
    let lines =
        [
            ("@var @free $Cîty =", [{TokenStartLocation = 1;TokenEndLocation = 4;Token = Var};{TokenStartLocation = 6;TokenEndLocation = 10;Token = Free};{TokenStartLocation = 12; TokenEndLocation = 16; Token = VariableName "Cîty"};{TokenStartLocation = 18; TokenEndLocation = 18; Token = Equals}])
            ("  \"Which city áre you writing about?\"", [{TokenStartLocation = 3;TokenEndLocation = 37;Token = QuotedString "Which city áre you writing about?"}])
            ("  {", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = OpenCurly}])
            ("    \"London\" [$Cöuntry = \"U.K.\"]", [{TokenStartLocation = 5;TokenEndLocation = 12;Token = QuotedString "London"};{TokenStartLocation = 14;TokenEndLocation = 14;Token = OpenBrace};{TokenStartLocation = 15;TokenEndLocation = 22;Token = VariableName("Cöuntry")};{TokenStartLocation = 24;TokenEndLocation = 24;Token = Equals};{TokenStartLocation = 26;TokenEndLocation = 31;Token = QuotedString "U.K."};{TokenStartLocation = 32;TokenEndLocation = 32;Token = CloseBrace}])
            ("    \"Berlin\" [$Country = \"Germany\" || %Gønder = \"Male\"]", [{TokenStartLocation = 5;TokenEndLocation = 12;Token = QuotedString "Berlin"};{TokenStartLocation = 14;TokenEndLocation = 14;Token = OpenBrace};{TokenStartLocation = 15;TokenEndLocation = 22;Token = VariableName("Country")};{TokenStartLocation = 24;TokenEndLocation = 24;Token = Equals};{TokenStartLocation = 26;TokenEndLocation = 34;Token = QuotedString "Germany"};{TokenStartLocation = 36;TokenEndLocation = 37;Token = Or};{TokenStartLocation = 39;TokenEndLocation = 45;Token=AttributeName("Gønder")};{TokenStartLocation = 47;TokenEndLocation = 47;Token = Equals};{TokenStartLocation = 49;TokenEndLocation = 54;Token = QuotedString "Male"};{TokenStartLocation = 55;TokenEndLocation = 55;Token = CloseBrace}])
            ("    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]", [{TokenStartLocation = 5;TokenEndLocation = 11;Token = QuotedString "Paris"};{TokenStartLocation = 13;TokenEndLocation = 13;Token = OpenBrace};{TokenStartLocation = 14;TokenEndLocation = 21;Token = VariableName("Country")};{TokenStartLocation = 23;TokenEndLocation = 24;Token = NotEquals};{TokenStartLocation = 26;TokenEndLocation = 34;Token = QuotedString "Germany"};{TokenStartLocation = 36;TokenEndLocation = 37;Token = And};{TokenStartLocation = 39;TokenEndLocation = 46;Token=VariableName("Country")};{TokenStartLocation = 48;TokenEndLocation = 49;Token = NotEquals};{TokenStartLocation = 51;TokenEndLocation = 56;Token = QuotedString "U.K."};{TokenStartLocation = 57;TokenEndLocation = 57;Token = CloseBrace}])
            ("  }", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = CloseCurly}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableOrAttributeLineTokenizer.tokenizeLine)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens

[<Test>]
let ``Bad variable lines``() =
    let lines =
        [
            ("    \"London", [{TokenStartLocation = 5;TokenEndLocation = 11;Token = InvalidUnrecognised "\"London"}])
        ]
    let tokens =
        lines
        |> Seq.map (fst >> VariableOrAttributeLineTokenizer.tokenizeLine)
    let expected =
        lines
        |> Seq.map snd
    test (lines |> Seq.map fst) expected tokens
