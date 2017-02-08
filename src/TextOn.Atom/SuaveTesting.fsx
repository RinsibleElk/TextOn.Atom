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

let request<'T> (url : string) (data: 'T)  = async {
    Logger.logf "Service" "Sending request: %O" [| data |]
    let r = System.Net.WebRequest.Create url
    let req: FunScript.Core.Web.WebRequest = unbox r
    req.Headers.Add("Accept", "application/json")
    req.Headers.Add("Content-Type", "application/json")
    req.Method <- "POST"

    let str = Globals.JSON.stringify data
    let data = System.Text.Encoding.UTF8.GetBytes str
    let stream = req.GetRequestStream()
    stream.Write (data, 0, data.Length )
    let! res = req.AsyncGetResponse ()
    let stream =  res.GetResponseStream()
    let data = System.Text.Encoding.UTF8.GetString stream.Contents
    let d = Globals.JSON.parse data
    let res = unbox<string[]>(d)
    return res }


