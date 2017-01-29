module TextOn.Atom.Test.TestAttributeLineTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

let test lines =
    let received =
        lines
        |> Seq.map (fst >> AttributeLineTokenizer.tokenizeLine >> List.ofSeq)
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
let ``Correctly formatted attribute definition``() =
    let lines =
        [
            ("@att %Gender", [{TokenStartLocation = 1;TokenEndLocation = 4;Token = Att};{TokenStartLocation = 6;TokenEndLocation = 12;Token = AttributeName "Gender"}])
            ("  {", [{TokenStartLocation = 3;TokenEndLocation = 3;Token = OpenCurly}])
            ("    \"Male\"", [{TokenStartLocation = 5;TokenEndLocation = 10;Token = QuotedString "Male"}])
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
