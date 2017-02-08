namespace TextOn.Atom

open System

[<AutoOpen>]
module Cprint =
    /// Colored printf
    let cprintf c fmt = 
        Printf.kprintf
            (fun s ->
                try
                    Console.ForegroundColor <- c
                    Console.Write s
                finally
                    Console.ResetColor())
            fmt

    /// Colored printfn
    let cprintfn c fmt =
        cprintf c fmt
        printfn ""
