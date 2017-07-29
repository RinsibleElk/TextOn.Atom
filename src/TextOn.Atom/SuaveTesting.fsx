#I "bin/Debug"
#I "../../packages/FSharp.Data/lib/net40"
#r "bin/Debug/TextOn.Atom.exe"
#r "FSharp.Data"
#r "Suave"
#r "Newtonsoft.Json"

open TextOn.Atom
open FSharp.Data
open System
open Suave
open Newtonsoft.Json
open Suave.Web

let cts = new CancellationTokenSource()
let conf = { defaultConfig with cancellationToken = cts.Token }
let listening, server = startWebServerAsync conf (OK "Hello World")
Async.Start(server, cts.Token)
printfn "Make requests now"
Console.ReadKey true |> ignore
cts.Cancel()
