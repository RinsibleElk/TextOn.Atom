module TextOn.Core.Test.Tokenizer

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

[<Test>]
let ``Test that this test binary works`` () =
    let a = [ 1 .. 10 ] |> List.sum
    ()
