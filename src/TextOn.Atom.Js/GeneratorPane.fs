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

    type IClipboard with
        [<FunScript.JSEmitInline("({0}.write({1}))")>]
        member __.write(s:string) : unit = failwith "JS"

let private td() =
    jq("<td />").addClass("textontablecell")

let private th() =
    jq("<th />").addClass("textontablecell")

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
let private tryFindTextOnGeneratorPane closeIfNotFound = 
    let panes = unbox<IPane[]> (Globals.atom.workspace.getPanes())
    [ for pane in panes do
          for item in pane.getItems() do
              if getTitle item = "TextOn Generator" then yield pane, item ]
    |> List.tryPick Some
    |> fun o ->
        if o.IsSome then
            o
        else
            if closeIfNotFound then LanguageService.generatorStop()
            None

/// Opens or activates the TextOn Generator panel
let private openTextOnGeneratorPane closeIfNotFound =
    Async.FromContinuations(fun (cont, econt, ccont) ->
        // Activate TextOn Generator and then switch back
        let prevPane = Globals.atom.workspace.getActivePane()
        let prevItem = prevPane.getActiveItem()
        let activateAndCont () = 
            prevPane.activate() |> ignore
            prevPane.activateItem(prevItem) |> ignore
            cont ()
        match tryFindTextOnGeneratorPane closeIfNotFound with
        | Some(pane, item) ->
            pane.activateItem(item) |> ignore
            pane.activate() |> ignore
            activateAndCont ()
        | None ->
            if (not closeIfNotFound) then
                Globals.atom.workspace
                  ._open("TextOn Generator", {split = "right"})
                  ._done((fun ed -> activateAndCont ()) |> unbox<Function>))

/// Get the cursor position, since from this, the app can figure out what it is the user wants to follow in the generator.
let private getTextOnCursorLine () =
    let editor = Globals.atom.workspace.getActiveTextEditor()
    let posn = editor.getCursorBufferPosition()
    (int posn.row)

/// Remove any whitespace and also the specified suffix from a string
let trimEnd (suffix:string) (text:string) = 
    let text = text.Trim()
    let text = if text.EndsWith(suffix) then text.Substring(0, text.Length-suffix.Length) else text
    text.Trim()

let navigateToFunction fileName functionName =
    async {
        let! navigationResult = LanguageService.navigateToFunction fileName functionName
        if navigationResult.IsSome then
            let data = navigationResult.Value.Data
            navigateToEditor data.FileName (data.LineNumber - 1) (data.Location - 1)
        return () }
    |> Async.StartImmediate
    |> box

let navigateToAttribute fileName attributeName =
    async {
        let! navigationResult = LanguageService.navigateToAttribute fileName attributeName
        if navigationResult.IsSome then
            let data = navigationResult.Value.Data
            navigateToEditor data.FileName (data.LineNumber - 1) (data.Location - 1)
        return () }
    |> Async.StartImmediate
    |> box

let navigateToVariable fileName variableName =
    async {
        let! navigationResult = LanguageService.navigateToVariable fileName variableName
        if navigationResult.IsSome then
            let data = navigationResult.Value.Data
            navigateToEditor data.FileName (data.LineNumber - 1) (data.Location - 1)
        return () }
    |> Async.StartImmediate
    |> box

let navigateToFileLine file line =
    async {
        navigateToEditor file (line - 1) 0 }
    |> Async.StartImmediate
    |> box

let private makeTitle (res:GeneratorData) =
    let link =
        jq("<a />")
            .append(res.FunctionName)
            .click(fun _ -> navigateToFunction res.FileName res.FunctionName)
    jq("<h1>Generator for </h1>").append(link)

let rec private valueSet ty (name:string) (value:string) =
    async {
        let! generatorData = LanguageService.generatorValueSet ty name value
        if generatorData.IsSome then
            let data = generatorData.Value.Data
            do! replaceTextOnGeneratorHtmlPanel data
        return () }
    |> Async.StartImmediate
    |> box

/// Actually do the generation.
and private performGeneration() =
    async {
        let! generatorData = LanguageService.generate()
        if generatorData.IsSome then
            let data = generatorData.Value.Data
            do! replaceTextOnGeneratorHtmlPanel data
        return () }
    |> Async.StartImmediate
    |> box

/// Actually do the generation.
and private copyToClipboard(output:OutputString[]) =
    async {
        Globals.atom.clipboard.write(output |> Array.map (fun o -> o.Value) |> Array.fold (+) "")
        return () }
    |> Async.StartImmediate
    |> box

and private makeCombobox ty name value options =
    let options =
        if value = "" then
            "" :: options
        else
            value :: ("" :: (options |> List.filter (fun o -> o <> value)))
    let q =
        options
        |> List.fold
            (fun (q:JQuery) o -> q.append("<option>" + o + "</option>"))
            (jq("<select id=\"" + name + "\" />"))
    q
        .change
            (fun o ->
                let value = unbox <| q._val()
                valueSet ty name value)

and private makeLinkForAttribute fileName (attribute:string) =
    jq("<a />")
        .append(attribute)
        .click(fun _ -> navigateToAttribute fileName attribute)

and private makeLinkForVariable fileName (variable:string) =
    jq("<a />")
        .append(variable)
        .click(fun _ -> navigateToVariable fileName variable)

and private makeLinkForLine file line (text:string) =
    jq("<a />")
        .append(text)
        .click(fun _ -> navigateToFileLine file line)

and private makeAttributes (res:GeneratorData) =
    let rows =
        res.Attributes
        |> Array.map (fun att -> jq("<tr />").append(td().append(makeLinkForAttribute res.FileName att.Name)).append(td().append(makeCombobox "Attribute" att.Name att.Value (att.Suggestions |> List.ofArray))))
    let topRow = jq("<tr />").append(th().append("Name")).append(th().append("Value"))
    let tbody =
        rows
        |> Array.fold
            (fun (q:JQuery) r -> q.append(r))
            (jq("<tbody />"))
    jq("<table />").append(jq("<thead />").append(topRow)).append(tbody)

and private makeVariables (res:GeneratorData) =
    let rows =
        res.Variables
        |> Array.map (fun variable -> jq("<tr />").append(td().append(makeLinkForVariable res.FileName variable.Name)).append(td().append(variable.Text)).append(td().append(makeCombobox "Variable" variable.Name variable.Value (variable.Suggestions |> List.ofArray))))
    let topRow = jq("<tr />").append(th().append("Name")).append(th().append("Text")).append(th().append("Value"))
    let tbody =
        rows
        |> Array.fold
            (fun (q:JQuery) r -> q.append(r))
            (jq("<tbody />"))
    jq("<table />").append(jq("<thead />").append(topRow)).append(tbody)

/// Replace contents of panel with HTML output 
and private replaceTextOnGeneratorHtmlPanel (res:GeneratorData) = async {
    jq(".texton").empty() |> ignore

    let identity() = "html" + string DateTime.Now.Ticks            

    let title = jq("<div />").addClass("content").append(makeTitle res)
    let paddedTitle = jq("<div class='inset-panel padded'/>").append(title)

    let attributes = jq("<div />").addClass("content").append("<h2>Attributes</h2>").append(makeAttributes res)
    let paddedAttributes = jq("<div class='inset-panel padded'/>").append(attributes)

    let variables = jq("<div />").addClass("content").append("<h2>Variables</h2>").append(makeVariables res)
    let paddedVariables = jq("<div class='inset-panel padded'/>").append(variables)

    let makePaddedGeneratorButton() =
        let button = jq("<button>Generate</button>").click(fun _ -> performGeneration())
        let generatorButton = jq("<div />").addClass("content").append("<h2>Generate</h2>").append(button)
        jq("<div class='inset-panel padded'/>").append(generatorButton)

    let makePaddedGeneratorOutput (output:OutputString[]) =
        let outputHtml =
            output
            |> Array.fold
                (fun (q:JQuery, li) t ->
                    if t.IsPb then (jq("<p />"), q::li)
                    else
                        (q.append(makeLinkForLine t.File t.LineNumber t.Value), li))
                (jq("<p />"), [])
            |> fun (q, li) -> (q::li) |> List.rev
            |> List.fold
                (fun (q:JQuery) q2 -> q.append(q2))
                (jq("<div />"))
        let paddedOutput = jq("<div />").addClass("content").append("<h2>Output</h2>").append(outputHtml)
        let copyToClipboardButton = jq("<button>Copy to Clipboard</button>").click(fun _ -> copyToClipboard output)
        let paddedCopyToClipboardButton = jq("<div />").addClass("content").append(copyToClipboardButton)
        jq("<div class='inset-panel padded'/>").append(paddedOutput).append(paddedCopyToClipboardButton)

    // Wrap the content with standard collapsible output panel.
    let q =
        jq("<atom-panel id='" + identity() + "' />").addClass("top texton-block texton-html-block")
            .append(jq("<div class='padded'/>").append(paddedTitle))
            .append(jq("<div class='padded'/>").append(paddedAttributes))
            .append(jq("<div class='padded'/>").append(paddedVariables))
    let q =
        if res.CanGenerate then
            q.append(jq("<div class='padded'/>").append(makePaddedGeneratorButton()))
        else
            q
    let q =
        if res.Output.Length > 0 then
            q.append(jq("<div class='padded'/>").append(makePaddedGeneratorOutput res.Output))
        else
            q
    q.appendTo(jq(".texton")) |> ignore }

/// Apend the result (Alt+Enter).
and private sendToGenerator (res:GeneratorData) = async {
    do! replaceTextOnGeneratorHtmlPanel res
    jq(".texton").scrollTop(99999999.) |> ignore }

/// Send the current line/file/selection to TextOn Generator
let private sendToTextOnGenerator closeIfNotFound = async {
    let editor = Globals.atom.workspace.getActiveTextEditor()
    if isTextOnEditor editor then
        // Get cursor position *before* opening TextOn Generator (it changes focus)
        let line = getTextOnCursorLine ()
        do! openTextOnGeneratorPane closeIfNotFound
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
            unbox <| fun () -> openTextOnGeneratorPane false |> Async.StartImmediate) |> ignore
        Globals.atom.workspace.addOpener(fun uri ->
            try 
                if uri.EndsWith "TextOn Generator" then box (newGeneratorPane ())
                else null
            with _ -> null)
        // Register commands that send some TextOn data to TextOn Generator.
        Globals.atom.commands.add(
            "atom-text-editor",
            "TextOn:Send-To-Generator",
            unbox <| fun () -> sendToTextOnGenerator false |> Async.StartImmediate) |> ignore

    member x.deactivate() =
        Logger.deactivate ()

