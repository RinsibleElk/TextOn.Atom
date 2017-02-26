namespace TextOn.Atom.Js

open System
open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open Control
open TextOn.Atom.DTO
open TextOn.Atom.DTO.DTO

[<ReflectedDefinition>]
module LanguageService =

    let genPort () =
        let r = Globals.Math.random ()
        let r' = r * (8999. - 8100.) + 8100.
        r'.ToString().Substring(0,4)

    let port = genPort ()

    let url s = sprintf @"http://localhost:%s/%s" port s
    // flag to send tooltip response to the proper event stream
    let mutable private toolbarFlag = false

    let mutable private service : ChildProcess option =  None

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
        return res
    }

    let tryParse<'T> s =
        try unbox<'T>(Globals.JSON.parse s) |> Some
        with ex -> None

    let private parseResponse<'T> (response : string[]) : DTO.Result<'T> option [] =
        response |> Array.map (fun s ->
          match tryParse<DTO.Result<'T>> s with
          | None -> Logger.logf "Service" "Invalid response from TextOn: %s" [| s |]; None
          | Some event ->
            let o = box event
            Logger.logf "Service" "Got '%s': %O" [| box event.Kind; o |]
            match event.Kind with
            | "project" | "errors" | "completion" | "symboluse" | "helptext"
            | "tooltip" | "finddecl" | "compilerlocation" | "lint" | "generatorSetup" | "navigate" -> Some event
            | "error" -> Logger.logf "Service" "Received error event '%s': %O" [| box s; o |]; None
            | "info" -> Logger.logf "Service" "Received info event '%s': %O" [| box s; o |]; None
            | s -> Logger.logf "Service" "Received unexpected event '%s': %O" [| box s; o |]; None)

    let send<'T> id req =
        async {
            try
                let! r = req
                return (r |> parseResponse<'T>).[id]
            with e ->
                Logger.logf "ERROR" "Parsing response failed: %O" [| e |]
                return None }

    let project s =
        {DTO.ProjectRequest.FileName = s}
        |> request (url "project")
        |> send<unit> 0

    let parse path (text : string) =
        let lines = text.Replace("\uFEFF", "").Split('\n')
        {DTO.ParseRequest.FileName = path; DTO.ParseRequest.Lines = lines; DTO.ParseRequest.IsAsync = true }
        |> request (url "parse")
        |> send<DTO.Error[]> 0

    let helptext s =
        {DTO.HelptextRequest.Symbol = s}
        |> request (url "helptext")
        |> send<DTO.Helptext> 0

    let parseEditor (editor : IEditor) =
        if isTextOnEditor editor && unbox<obj>(editor.buffer.file) <> null then
            let path = editor.buffer.file.path
            let text = editor.getText()
            parse path text
        else
            async { return None}

    let completion fn content line col =
        {CompletionRequest.Line = line; FileName = fn; SourceLine =content; Column = col; Filter = ""}
        |> request (url "completion")
        |> send<Completion[]> 1

    let symbolUse fn line col =
        {PositionRequest.Line = line; FileName = fn; Column = col; Filter = ""}
        |> request (url "symboluse")
        |> send<SymbolUses> 0

    let tooltip fn line col =
        toolbarFlag <- false
        {PositionRequest.Line = line; FileName = fn; Column = col; Filter = ""}
        |> request (url "tooltip")
        |> send<OverloadSignature[][]> 0

    let toolbar fn line col =
        toolbarFlag <- true
        {PositionRequest.Line = line; FileName = fn; Column = col; Filter = ""}
        |> request (url "tooltip")
        |> send<OverloadSignature[][]> 0

    let findDeclaration fn line col =
        {PositionRequest.Line = line; FileName = fn; Column = col; Filter = ""}
        |> request (url "finddeclaration")
        |> send<Declaration> 0

    let lint editor =
        if isTextOnEditor editor && unbox<obj>(editor.buffer.file) <> null then
            {LintRequest.FileName = editor.buffer.file.path }
            |> request (url "lint")
            |> send<LintWarning[]> 0
         else async {return None}

    let generatorStart editor lineNumber =
        if isTextOnEditor editor && unbox<obj>(editor.buffer.file) <> null then
            let path = editor.buffer.file.path
            let text = editor.getText()
            let lines = text.Replace("\uFEFF", "").Split('\n')
            {DTO.GeneratorStartRequest.FileName = path; DTO.GeneratorStartRequest.Lines = lines; DTO.GeneratorStartRequest.LineNumber = lineNumber }
            |> request (url "generatorstart")
            |> send<DTO.GeneratorData> 0
         else async {return None}

    let navigateToFunction fileName functionName =
        { DTO.NavigateRequest.FileName = fileName ; DTO.NavigateRequest.NavigateType = "NavigateToFunction" ; DTO.NavigateRequest.Name = functionName }
        |> request (url "navigaterequest")
        |> send<DTO.NavigateData> 0

    let navigateToAttribute fileName attributeName =
        { DTO.NavigateRequest.FileName = fileName ; DTO.NavigateRequest.NavigateType = "NavigateToAttribute" ; DTO.NavigateRequest.Name = attributeName }
        |> request (url "navigaterequest")
        |> send<DTO.NavigateData> 0

    let navigateToVariable fileName variableName =
        { DTO.NavigateRequest.FileName = fileName ; DTO.NavigateRequest.NavigateType = "NavigateToVariable" ; DTO.NavigateRequest.Name = variableName }
        |> request (url "navigaterequest")
        |> send<DTO.NavigateData> 0

    let generatorValueSet ty name value =
        { DTO.GeneratorValueSetRequest.Type = ty ; DTO.GeneratorValueSetRequest.Name = name ; DTO.GeneratorValueSetRequest.Value = value }
        |> request (url "generatorvalueset")
        |> send<DTO.GeneratorData> 0

    let generatorStop () =
        { DTO.GeneratorStopRequest.Blank = "" }
        |> request (url "generatorstop")
        |> send<unit> 0
        |> ignore

    let generate () =
        let config : GeneratorConfiguration =
            {
                NumSpacesBetweenSentences = Globals.atom.config.get "texton.GeneratorConfig.NumSpacesBetweenSentences" |> unbox
                NumBlankLinesBetweenParagraphs = Globals.atom.config.get "texton.GeneratorConfig.NumBlankLinesBetweenParagraphs" |> unbox
                WindowsLineEndings = Globals.atom.config.get "texton.GeneratorConfig.WindowsLineEndings" |> unbox
            }
        { DTO.GenerateRequest.Config = config }
        |> request (url "generate")
        |> send<DTO.GeneratorData> 0

    let start () =
        try
            let location = TextOnProcess.textonPath ()
            if location = null then ()
            else
                let child = TextOnProcess.spawn location (TextOnProcess.fromPath "mono") ("--port " + port)
                service <- Some child
                child.stderr.on("data", unbox<Function>( fun n -> Globals.console.error (n.ToString()))) |> ignore
                ()
        with
        | exc ->
            Globals.console.error exc
            service <- None
            let opt = createEmpty<INotificationsOptions> ()
            opt.detail <- "Language services could not be spawned"
            opt.dismissable <- true
            Globals.atom.notifications.addError("Critical error", opt) |> ignore

    let stop () =
        service |> Option.iter (fun n -> n.kill "SIGKILL")
        service <- None
