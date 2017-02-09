namespace TextOn.Atom.Js

open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open TextOn.Atom.Js.Control

[<ReflectedDefinition>]
module ErrorLinterProvider =
    type Provider = {
        grammarScopes : string[]
        scope         : string
        lintOnFly     : bool
        lint          : IEditor -> Promise.Promise  }

    type LintResult = {
        ``type`` : string
        text     : string
        filePath : string
        range    : float[][]
    }

    let mapError (editor : IEditor) (item : DTO.Error)  =
        let range = [|[|float (item.StartLine - 1); float (item.StartColumn - 1)|];
                      [|float (item.EndLine - 1);  float (item.EndColumn)|]|]
        let error =
            {   ``type`` = item.Severity
                text = item.Message.Replace("\n", "")
                filePath = editor.buffer.file.path
                range = range
            }
        Logger.logf "Service" "Got error %A" [|error|]
        error :> obj

    let mapLint (editor : IEditor)  (item : DTO.LintWarning) =
        let range = [|[|float (item.StartLine - 1); float (item.StartColumn - 1)|];
                        [|float (item.EndLine - 1);  float (item.EndColumn)|]|]
        { ``type`` = "Trace"
          text = item.Info.Replace("\n", "")
          filePath = editor.buffer.file.path
          range = range
        } :> obj 

    let lint (editor : IEditor) =
        async {
            let! result = LanguageService.parseEditor editor
            let result' : DTO.LintWarning[] DTO.Result option = None
            let linter = Globals.atom.config.get("texton.UseLinter") |> unbox<bool>
            return
                match result, result' with
                | Some n, Some n' ->
                    let r = n.Data |> Array.map (mapError editor)
                    let r' = if linter then n'.Data |> Array.map (mapLint editor) else [||]
                    Array.concat [r; r']
                | Some n, None -> n.Data |> Array.map (mapError editor)
                | None, Some n -> if linter then n.Data |> Array.map (mapLint editor) else [||]
                | None, None -> [||]
        } |> Async.StartAsPromise

    let create () = [| { grammarScopes = [|"source.texton"|]; scope = "file"; lint = lint; lintOnFly = true} |]
