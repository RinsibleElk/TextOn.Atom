[<ReflectedDefinition>]
module TextOn.Atom.Js.GeneratorPane

open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open FunScript.TypeScript.path
open System
open TextOn.Atom.DTO.DTO

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
            Globals.atom.workspace
              ._open("TextOn Generator", {split = "right"})
              ._done((fun ed -> activateAndCont ()) |> unbox<Function>))

/// Get the cursor position, since from this, the app can figure out what it is the user wants to follow in the generator.
let private getTextOnCursorLine () =
    let editor = Globals.atom.workspace.getActiveTextEditor()
    let posn = editor.getCursorBufferPosition()
    (int posn.row) + 1

/// Remove any whitespace and also the specified suffix from a string
let trimEnd (suffix:string) (text:string) = 
    let text = text.Trim()
    let text = if text.EndsWith(suffix) then text.Substring(0, text.Length-suffix.Length) else text
    text.Trim()

/// Fire and forget navigate to a function from a link in the pane.
let navigateToFunction fileName functionName =
    async {
        let! navigationResult = LanguageService.navigateToFunction fileName functionName
        if navigationResult.IsSome then
            let data = navigationResult.Value.Data
            navigateToEditor data.FileName data.LineNumber data.Location
        return () }
    |> Async.StartImmediate
    |> box

let private makeTitle (res:GeneratorResult) =
    let link =
        jq("<a />")
            .append(res.functionName)
            .click(fun _ -> navigateToFunction res.fileName res.functionName)
    jq("<h1>Generator for </h1>").append(link)

/// Replace contents of panel with HTML output 
let private replaceTextOnGeneratorHtmlPanel expanded (res:GeneratorResult) = async {
    jq(".texton").empty() |> ignore

    let identity() = "html" + string DateTime.Now.Ticks            

    let! title = 
      async {
          // Just paste the content in a <div> together with all styles and such
          let el = jq("<div />").addClass("content").append(makeTitle res)
          return el }

    // Wrap the content with standard collapsible output panel.
    let paddedTitle = jq("<div class='inset-panel padded'/>").append(title)
    jq("<atom-panel id='" + identity() + "' />").addClass("top texton-block texton-html-block")
      .append(jq("<div class='padded'/>").append(paddedTitle))
      .appendTo(jq(".texton")) |> ignore }

/// Apend the result (Alt+Enter).
let private sendToGenerator (res:GeneratorData) = async {
    let html = { fileName = res.FileName ; functionName = res.FunctionName }
    do! replaceTextOnGeneratorHtmlPanel true html
    jq(".texton").scrollTop(99999999.) |> ignore }

/// Send the current line/file/selection to TextOn Generator
let private sendToTextOnGenerator () = async {
    let editor = Globals.atom.workspace.getActiveTextEditor()
    if isTextOnEditor editor then
        // Get cursor position *before* opening TextOn Generator (it changes focus)
        let line = getTextOnCursorLine ()
        do! openTextOnGeneratorPane()
        let! res = LanguageService.generatorStart editor line
        if res.IsSome then
            do! sendToGenerator res.Value.Data }

[<ReflectedDefinition>]
type TextOnGenerator() =
    member x.activate(state:obj) =
        Logger.activate "TextOnGenerator"

        // Register command to open TextOn Generator & handler that loads the TextOn Generator panel GUI
        Globals.atom.commands.add(
            "atom-workspace",
            "TextOn:Open-Generator",
            unbox <| fun () -> openTextOnGeneratorPane () |> Async.StartImmediate) |> ignore
        Globals.atom.workspace.addOpener(fun uri ->
            try 
                if uri.EndsWith "TextOn Generator" then box (newGeneratorPane ())
                else null
            with _ -> null)
        // Register commands that send some TextOn code to TextOn Generator.
        Globals.atom.commands.add(
            "atom-text-editor",
            "TextOn:Send-To-Generator",
            unbox <| fun () -> sendToTextOnGenerator() |> Async.StartImmediate) |> ignore

    member x.deactivate() =
        Logger.deactivate ()

