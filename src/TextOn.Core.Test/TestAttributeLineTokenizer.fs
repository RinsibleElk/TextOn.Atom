module TextOn.Core.Test.TestAttributeLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core.Tokenizing

let private test lines =
    let received =
        lines
        |> Seq.map (fst >> VariableOrAttributeLineTokenizer.tokenizeLine)
    let expected =
        lines
        |> Seq.map snd
    let source = lines |> Seq.map fst
    Seq.zip3
        source
        expected
        received
    |> Seq.iter
        (fun (s, e, r) ->
            let eLen = e |> List.length
            let rLen = r |> List.length
            let eTrun = e |> Seq.truncate rLen |> Seq.toList
            let rTrun = r |> Seq.truncate eLen |> Seq.toList
            let firstIncorrect = List.zip eTrun rTrun |> List.tryFind (fun (e,r) -> e <> r) |> Option.map (fun (e,r) -> sprintf "%A\n\n<>\n\n%A" e r)
            if eLen > rLen then
                failwithf "Too short for %s - got %d tokens, expected %d tokens, first missing token %A, first incorrect token %A" s rLen eLen (e |> Seq.skip (rLen) |> Seq.head) firstIncorrect
            else if eLen < rLen then
                failwithf "Too long for %s - got %d tokens, expected %d tokens, first extra token %A, first incorrect token %A" s rLen eLen (r |> Seq.skip (eLen) |> Seq.head) firstIncorrect
            else if firstIncorrect.IsSome then
                failwithf "Incorrect token for %s %s" s firstIncorrect.Value)

[<Test>]
let ``Correctly formatted attribute definition``() =
    let lines =
        [
            ("@att %Gender = \"What is the gender of your target audience?\"", [{TokenStartLocation = 1;TokenEndLocation = 4;Token = Att};{TokenStartLocation = 6;TokenEndLocation = 12;Token = AttributeName "Gender"};{TokenStartLocation = 14;TokenEndLocation = 14;Token = Equals};{TokenStartLocation = 16;TokenEndLocation = 60;Token = QuotedString "What is the gender of your target audience?"}])
            ("  {", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = OpenCurly}])
            ("    \"Male\" // Comment after no condition", [{TokenStartLocation = 5;TokenEndLocation = 10;Token = QuotedString "Male"}])
            ("    \"Female\"", [{TokenStartLocation = 5;TokenEndLocation = 12;Token = QuotedString "Female"}])
            ("  }", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = CloseCurly}])
        ]
    test lines

[<Test>]
let ``Quoted string with escaped characters in it``() =
    let lines =
        [
            ("    \"This string has a \\\"quote\\\" in it.  \"", [{TokenStartLocation = 5;TokenEndLocation = 42;Token = QuotedString "This string has a \"quote\" in it.  "}])
            ("    \"This string has a \\\\ in it.  \"", [{TokenStartLocation = 5;TokenEndLocation = 35;Token = QuotedString "This string has a \\ in it.  "}])
        ]
    test lines
