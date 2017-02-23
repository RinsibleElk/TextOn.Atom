[<ReflectedDefinition>]
module TextOnIDE

open FunScript
open FunScript.TypeScript
open FunScript.TypeScript.fs
open FunScript.TypeScript.child_process
open FunScript.TypeScript.AtomCore
open FunScript.TypeScript.text_buffer
open FunScript.TypeScript.path
open TextOn.Atom.Js
open TextOn.Atom.Js.Control
open TextOn.Atom.Js.GeneratorPane

[<AutoOpen>]
module TextOnCommands =

    let openSettings() =
        // sometimes this will crash if settings-view hasn't been opened already
        Globals.atom.workspace._open ("atom://config/packages/texton", ())


type TextOnIDE() =
    let subscriptions = ResizeArray()
    let generator = TextOnGenerator()

//    member __.provide () =
//        [| AutocompleteProvider.create() |]

    member __.provideErrors () =
        ErrorLinterProvider.create ()

//    member __.getSuggestion(options : CompletionHelpers.GetSuggestionOptions) =
//        [| AutocompleteProvider.getSuggestion options|]

    member x.activate(state:obj) =

//        let show = Globals.atom.config.get("ionide-fsharp.ShowQuickInfoPanel") |> unbox<bool>
//        let highlight = Globals.atom.config.get("ionide-fsharp.ShowUseHighlights") |> unbox<bool>
        let debug = Globals.atom.config.get("texton.DeveloperMode") |> unbox<bool>

        if debug then Logger.activate ("TextOn IDE")
        LanguageService.start ()
        generator.activate()
//        Parser.activate ()
//        TooltipHandler.activate ()
//        if show then ToolbarHandler.activate()
//        FindDeclaration.activate ()
//        if highlight then HighlightUse.activate ()
//        FormatHandler.activate ()

        // Subscriptions to monitor whether F# IDE functionality should be activated
//        Globals.atom.config.onDidChange("ionide-fsharp.ShowQuickInfoPanel", fun n -> 
//          if n.newValue then ToolbarHandler.activate() 
//          else ToolbarHandler.deactivate() ) |> subscriptions.Add

//        Globals.atom.config.onDidChange("ionide-fsharp.ShowUseHighlights", fun n -> 
//          if n.newValue then HighlightUse.activate() 
//          else HighlightUse.deactivate() ) |> subscriptions.Add

        Globals.atom.config.onDidChange("texton.DeveloperMode", fun n -> 
          if n.newValue then Logger.activate("TextOn IDE") 
          else Logger.deactivate()  ) |> subscriptions.Add

        // Commands that will be accessible through the Atom command palette
        Globals.atom.commands.add("atom-workspace", "TextOn:Settings",openSettings |> unbox<Function>) |> ignore

    member x.deactivate() =
        subscriptions |> Seq.iter(fun n -> n.dispose())
        subscriptions.Clear()
        generator.deactivate()
//        let show = Globals.atom.config.get("ionide-fsharp.ShowQuickInfoPanel") |> unbox<bool>

//        Parser.deactivate ()
//        TooltipHandler.deactivate ()
//        if show then ToolbarHandler.deactivate()
//        FindDeclaration.deactivate ()
//        HighlightUse.deactivate ()
        LanguageService.stop ()
        Logger.deactivate ()
