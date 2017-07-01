module TextOn.Core.Test.TestIdentifierTokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Core

[<Test>]
let ``Valid token``() =
    let line = "    Hello world. [%Blah = \"Value\"]"
    test <@ IdentifierTokenizer.findLengthOfWord 19 (line.Length - 1) line = 4 @>

[<Test>]
let ``Invalid token``() =
    let line = "    Hello world. [% = \"Value\"]"
    test <@ IdentifierTokenizer.findLengthOfWord 19 (line.Length - 1) line = 0 @>


