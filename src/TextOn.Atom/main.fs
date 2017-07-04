namespace TextOn.Atom

open System
open System.IO
open TextOn.ArgParser

type Mode =
    | [<ArgDescription("Interactive mode (compile a full template and fill in values for generation)")>] Interactive of InteractiveConfig
    | [<ArgDescription("Server mode (listen on a given port and provide IDE services to a UI over HTTP)")>] Server of ServerModeConfig

module internal Main =
    [<EntryPoint>]
    let main argv =
        let mode : Mode option = ArgParser.tryParse argv
        if mode.IsNone then
            0
        else
            match mode.Value with
            | Interactive interactiveConfig -> RunInteractive.run interactiveConfig
            | Server serverConfig -> RunServer.run serverConfig
