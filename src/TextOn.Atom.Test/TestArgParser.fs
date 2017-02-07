module TextOn.Atom.Test.TestArgParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote

open TextOn.Atom

type X = {
    A : int
    B : double
    C : string }

type Y = {
    Not : int
    Present : double }

type U =
    | X of X
    | B of Y

type E = | Abc | Def

type F = { V : E }
type G = { W : E option }
type H = { Y : E list }

let mapC2 f c =
    match c with
    | Choice1Of2 a -> Choice1Of2 a
    | Choice2Of2 b -> Choice2Of2 (f b)

let runTest<'a when 'a : equality> args expected =
    let result = ArgParser.parseOrError<'a> args |> mapC2 (fun x -> x.ToString())
    match (result, expected) with
    | (Choice2Of2 e1, Choice2Of2 (e2:string)) -> test <@ e1.Split([|'\n';'\r'|], StringSplitOptions.RemoveEmptyEntries) = e2.Split([|'\n';'\r'|], StringSplitOptions.RemoveEmptyEntries) @>
    | (Choice1Of2 a1, Choice1Of2 a2) -> test <@ a1 = a2 @>
    | _ -> failwithf "Didn't match %A <> %A" result expected

[<Test>]
let ``Fail to parse simple record``() =
    let error =
        "--a - Not supplied
--b - Not supplied
--c - Not supplied
{
  --a <argument> (Integer) : A
  --b <argument> (Float) : B
  --c <argument> (String) : C
}"
    runTest<X> [||] (Choice2Of2 error)

[<Test>]
let ``Successfully parse simple record``() =
    let expected = Choice1Of2 { A = 1 ; B = 2.0 ; C = "Hello" }
    runTest [|"--b";"2";"--c";"Hello";"--a";"1"|] expected

[<Test>]
let ``Successfully parse union``() =
    let expected = Choice1Of2 (X { A = 1 ; B = 2.0 ; C = "Hello" })
    runTest [|"--b";"2";"--c";"Hello";"--a";"1"|] expected

[<Test>]
let ``Successfully parse simple union``() =
    let expected = Choice1Of2 { V = Abc }
    runTest [|"--v";"Abc"|] expected

[<Test>]
let ``Successfully parse simple union option``() =
    let expected = Choice1Of2 { W = Some Abc }
    runTest [|"--w";"Abc"|] expected

[<Test>]
let ``Successfully parse simple union list``() =
    let expected = Choice1Of2 { Y = [Abc;Def] }
    runTest [|"--y";"Abc";"--y";"Def"|] expected

[<Test>]
let ``Successfully parse simple union option - no value``() =
    let expected = Choice1Of2 { W = None }
    runTest [||] expected

[<Test>]
let ``Successfully parse simple union list - no value``() =
    let expected = Choice1Of2 { Y = [] }
    runTest [||] expected
