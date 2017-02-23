[<ReflectedDefinition>]
module TextOn.Atom.Js.GeneratorPane

open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open FunScript.TypeScript.path

[<AutoOpen>]
module Bindings =
    [<JSEmitInline("new GeneratorPane()")>]
    let newGeneratorPane () : FunScript.TypeScript.atom.ScrollView = failwith "JS"

    [<JSEmitInline("{0}.getTitle()")>]
    let getTitle (item:obj) :string = failwith "JS"

    type IWorkspace with
        [<FunScript.JSEmitInline("({0}.addOpener({1}))")>]
        member __.addOpener(cb: string -> obj) : unit = failwith "JS"
        [<FunScript.JSEmitInline("({0}.paneForItem({1}))")>]
        member __.paneForItem(o : obj) : obj = failwith "JS"

/// Go to an opened editor with the specified file or open a new one
let private navigateToEditor file line col =
    // Try to go to an existing opened editor
    // In theory, `workspace.open` does this automatically, but in
    // reality, it does not work when the item is in another panel :(
    let mutable found = false
    for pane in unbox<IPane[]> (Globals.atom.workspace.getPanes() ) do
        if (not found) then
            for item in pane.getItems() do
                if (not found) then
                    try 
                        let ed = unbox<IEditor> item
                        if ed.getPath() = file then 
                            pane.activate()
                            pane.activateItem(ed) |> ignore
                            ed.setCursorBufferPosition [| line; col |] |> ignore
                            found <- true
                    with _ -> ()
    // If it did not exist, open a new one
    if not found then
        Globals.atom.workspace._open(file, {initialLine=line; initialColumn=col}) |> ignore

/// Find the "TextOn Generator" panel
let private tryFindTextOnGeneratorPane () = 
    let panes = unbox<IPane[]> (Globals.atom.workspace.getPanes())
    [ for pane in panes do
          for item in pane.getItems() do
              if getTitle item = "TextOn Generator" then yield pane, item ]
    |> List.tryPick Some

/// Helper JS mapping for the function below
type MessageEvent = { data:string }

/// A handler for messages sent by <iframe> elements that HTML output may put into FSI window
/// (a message "height <id> <number>" means max-height of iframe #<id> is given number) 
let private setupIFrameResizeHandler () = 
    Globals.window.addEventListener("message", fun e ->
      let data = (unbox e).data.Split(' ') |> List.ofSeq
      match data with
      | [ "height"; id; hgt ] -> 
          let hgt = if float hgt > 500.0 then 500.0 else float hgt
          jq("#" + id + " iframe").height(string hgt + "px") |> ignore
          jq(".textongen").scrollTop(99999999.) |> ignore
      | data -> Logger.logf "TextOn Generator" "Unhandled window message: %O" [| data |] )

/// Opens or activates the TextOn Generator panel
let private openTextOnGeneratorPane () =
    Async.FromContinuations(fun (cont, econt, ccont) ->
        // Activate TextOn Generator and then switch back
        let prevPane = Globals.atom.workspace.getActivePane()
        let prevItem = prevPane.getActiveItem()
        let activateAndCont () = 
            prevPane.activate() |> ignore
            prevPane.activateItem(prevItem) |> ignore
            cont ()
        match tryFindTextOnGeneratorPane () with
        | Some(pane, item) ->
            pane.activateItem(item) |> ignore
            pane.activate() |> ignore
            activateAndCont ()
        | None ->
            setupIFrameResizeHandler ()
            Globals.atom.workspace
              ._open("TextOn Generator", {split = "right"})
              ._done((fun ed -> activateAndCont ()) |> unbox<Function>))

/// Send the current line/file/selection to TextOn Generator
let private sendToTextOnGenerator () = async {
    let editor = Globals.atom.workspace.getActiveTextEditor()
    if isTextOnEditor editor then
        // Get selection *before* opening TextOn Generator (it changes focus)
        do! openTextOnGeneratorPane() }

[<ReflectedDefinition>]
type TextOnGenerator() =
    member x.activate(state:obj) =
        Logger.activate "TextOnGenerator"

        // Register command to open TextOn Generator & handler that loads the TextOn Generator panel GUI
        Globals.atom.commands.add(
            "atom-workspace",
            "TextOn:Open-Generator",
            unbox (fun () -> Async.StartImmediate (openTextOnGeneratorPane ()))) |> ignore
        Globals.atom.workspace.addOpener(fun uri ->
            try 
                if uri.EndsWith "TextOn Generator" then box (newGeneratorPane ())
                else null
            with _ -> null)

    member x.deactivate() =
        Logger.deactivate ()

