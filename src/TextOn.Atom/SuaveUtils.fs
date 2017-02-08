namespace TextOn.Atom

open System.IO
open System.Collections.Concurrent
open System.Diagnostics
open System
open Suave
open Newtonsoft.Json

type Serializer = obj -> string
type SourceFilePath = string
type LineStr = string

type Result<'a> =
  | Success of 'a
  | Failure of string

type Pos = 
    { Line: int
      Col: int }

[<RequireQualifiedAccess>]
module SuaveUtils =
    let normalizePath (file : string) = 
        if file.EndsWith ".texton" then 
            let p = Path.GetFullPath file
            (p.Chars 0).ToString().ToLower() + p.Substring(1)
        else file
    let private fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>
    let inline debug msg = Printf.kprintf Debug.WriteLine msg
    let inline fail msg = Printf.kprintf Debug.Fail msg

[<AutoOpen>]
module SuaveExtensions =
    type ConcurrentDictionary<'key, 'value> with
        member x.TryFind key =
            match x.TryGetValue key with
            | true, value -> Some value
            | _ -> None

        member x.ToSeq() =
            x |> Seq.map (fun (KeyValue(k, v)) -> k, v)

