namespace TextOn.Atom.Test

open System
open System.Diagnostics
open System.Threading
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open Suave
open Suave.Logging
open Suave.Logging.Message
open Suave.Http
open TextOn.Atom
open TextOn.Atom.DTO.DTO

type SuaveTestCtx(cts, suaveConfig, handler, client) =
    member __.Cts : CancellationTokenSource = cts
    member __.SuaveConfig : SuaveConfig = suaveConfig
    member __.Handler : HttpClientHandler = handler
    member __.Client : HttpClient = client
    interface IDisposable
        with
            member __.Dispose() =
                cts.Cancel()
                cts.Dispose()
                client.Dispose()
                handler.Dispose()

[<RequireQualifiedAccess>]
module TestSuave =
    let private createHandler() =
        let handler = new Net.Http.HttpClientHandler(AllowAutoRedirect = false)
        handler.AutomaticDecompression <- Net.DecompressionMethods.None
        handler

    let private createClient handler =
        new HttpClient(handler)

    let private runWithFactory factory config webParts : SuaveTestCtx =
        let binding = config.bindings.Head
        let baseUri = binding.ToString()
        let cts = new CancellationTokenSource()
        let config2 = { config with cancellationToken = cts.Token; bufferSize = 128; maxOps = 10 }
        let listening, server = factory config webParts
        Async.Start(server, cts.Token)
        listening |> Async.RunSynchronously |> ignore // wait for the server to start listening
        let handler     = createHandler()
        let client      = createClient handler
        new SuaveTestCtx(cts, config2, handler, client)

    let private runWith config webParts = runWithFactory startWebServerAsync config webParts

    let setUpTest() =
        let (config, app) = RunServer.makeServerConfig { Port = 7500 }
        runWith config (app)

    let private endpointUri (suaveConfig : SuaveConfig) =
        Uri(suaveConfig.bindings.Head.ToString())

    let private reqResp<'req, 'rsp> n (req:'req) (ctx:SuaveTestCtx) =
        let jsonObject  = JsonSerializer.writeJson req
        use content     = new StringContent(jsonObject, Text.Encoding.UTF8, "application/json")
        let uriBuilder   = UriBuilder (endpointUri ctx.SuaveConfig)
        uriBuilder.Path  <- "/" + n
        let url         = uriBuilder.Uri
        use result      = ctx.Client.PostAsync(url, content).Result
        result.Content.ReadAsStringAsync().Result
        |> JsonSerializer.readJson<string[]>
        |> Array.map (JsonSerializer.readJson<'rsp Result>)
        |> List.ofArray

    let sendParseRequest<'t> (file:string) (lines:string list) ctx =
        reqResp<ParseRequest, 't> "parse" { FileName = file ; Lines = lines |> List.toArray } ctx
