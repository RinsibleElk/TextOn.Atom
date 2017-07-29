module TextOn.Atom.Test.TestRunServer

open System
open System.Net.Http
open System.Net.Http.Headers
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic
open TextOn.Atom
open TextOn.Core.Conditions
open TextOn.Core.Linking
open Suave.Http
open Suave.Json
open TextOn.Atom.DTO.DTO

[<Test>]
let ``Test parse error``() =
    use tempDirs = new TestTempDir()
    let lines =
        [
            @"@blah"
            @"@func @main"
            @"{"
            @"  Hello world."
            @"}"
        ]
    let file = "example.texton"
    tempDirs.CreateFile file lines
    use ctx = TestSuave.setUpTest()
    let response = TestSuave.sendParseRequest<Error> file lines ctx
    Assert.AreEqual(1, response.Length)
    let rsp = response |> List.head
    Assert.AreEqual("errors", rsp.Kind)
    Assert.AreEqual(1, rsp.Data.Length)
    let data = rsp.Data.[0]
    Assert.AreEqual("Unrecognised starting token", data.text)
