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

    member __.provideErrors () =
        ErrorLinterProvider.create ()

    member x.activate(state:obj) =

        let debug = Globals.atom.config.get("texton.DeveloperMode") |> unbox<bool>

        if debug then Logger.activate ("TextOn IDE")
        LanguageService.start ()
        generator.activate()

        Globals.atom.config.onDidChange("texton.DeveloperMode", fun n -> 
          if n.newValue then Logger.activate("TextOn IDE") 
          else Logger.deactivate()  ) |> subscriptions.Add

        // Commands that will be accessible through the Atom command palette
        Globals.atom.commands.add("atom-workspace", "TextOn:Settings",openSettings |> unbox<Function>) |> ignore

    member x.deactivate() =
        subscriptions |> Seq.iter(fun n -> n.dispose())
        subscriptions.Clear()
        generator.deactivate()
        LanguageService.stop ()
        Logger.deactivate ()
