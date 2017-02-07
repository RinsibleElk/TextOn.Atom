namespace TextOn.Atom

open System
open System.IO

type ServerConfig =
    {
        [<ArgDescription("The port to listen on.")>]
        [<ArgRange(8100, 8999)>]
        Port : int
    }

[<RequireQualifiedAccess>]
module internal RunServer =
    let run serverConfig =
        let port = serverConfig.Port
        0
