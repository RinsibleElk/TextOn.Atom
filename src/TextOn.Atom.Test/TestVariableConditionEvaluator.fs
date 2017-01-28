module TextOn.Atom.Test.TestVariableConditionEvaluator

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
    test <@ VariableConditionEvaluator.resolve Map.empty Map.empty VarTrue @>

[<Test>]
let ``AreEqual success``() =
    test <@ VariableConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) Map.empty (VarAreEqual(Attribute 1, "Hello")) @>

[<Test>]
let ``AreNotEqual failure``() =
    test <@ VariableConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) Map.empty (VarAreNotEqual(Attribute 1, "Hello")) |> not @>

[<Test>]
let ``Either doesn't evaluate second``() =
    test <@ VariableConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) Map.empty (VarEither(VarAreEqual(Attribute 1, "Hello"), VarAreEqual(Attribute 712, "This will fail if evaluated"))) @>

[<Test>]
let ``Both does evaluate second``() =
    raises<KeyNotFoundException> <@ VariableConditionEvaluator.resolve ([(1,"Hello")] |> Map.ofSeq) Map.empty (VarBoth(VarAreEqual(Attribute 1, "Hello"), VarAreEqual(Attribute 712, "This will fail if evaluated"))) @>

[<Test>]
let ``AreEqual success on variable``() =
    test <@ VariableConditionEvaluator.resolve Map.empty ([(1,"Hello")] |> Map.ofSeq) (VarAreEqual(Variable 1, "Hello")) @>

[<Test>]
let ``AreNotEqual failure on variable``() =
    test <@ VariableConditionEvaluator.resolve Map.empty ([(1,"Hello")] |> Map.ofSeq) (VarAreNotEqual(Variable 1, "Hello")) |> not @>

