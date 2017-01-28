module TextOn.Atom.Test.TestConditionEvaluator

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

[<Test>]
let ``Unconditional returns true``() =
    test <@ ConditionEvaluator.resolve Map.empty True @>

[<Test>]
let ``AreEqual success``() =
    test <@ ConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) (AreEqual(1, "Hello")) @>

[<Test>]
let ``AreNotEqual failure``() =
    test <@ ConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) (AreNotEqual(1, "Hello")) |> not @>

[<Test>]
let ``Either doesn't evaluate second``() =
    test <@ ConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) (Either(AreEqual(1, "Hello"), AreEqual(712, "This will fail if evaluated"))) @>

[<Test>]
let ``Both does evaluate second``() =
    raises<KeyNotFoundException> <@ ConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) (Both(AreEqual(1, "Hello"), AreEqual(712, "This will fail if evaluated"))) @>

