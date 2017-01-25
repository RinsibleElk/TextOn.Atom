#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

type Message<'a,'b> =
    | Quit
    | NewData of 'a Async
    | Fetch of 'b option AsyncReplyChannel
type Agent<'a,'b> = Message<'a,'b> MailboxProcessor
[<RequireQualifiedAccess>]
module Agent =
    let map (f:'a Async -> 'b Async) (next:Agent<'b,'c>) =
        Agent.Start
            (fun inbox ->
                let mutable b = None
                let rec loop() =
                    async {
                        let! msg = inbox.Receive()
                        match msg with
                        | Quit -> return ()
                        | NewData a ->
                            b <- Some (f a |> Async.RunSynchronously)
                            next.Post(NewData(async { return b.Value }))
                            return! loop()
                        | Fetch(reply) ->
                            reply.Reply(b)
                            return! loop() }
                loop())
    let make() =
        Agent.Start
            (fun inbox ->
                let mutable b = None
                let rec loop() =
                    async {
                        let! msg = inbox.Receive()
                        match msg with
                        | Quit -> return ()
                        | NewData a ->
                            b <- Some a
                            return! loop()
                        | Fetch(reply) ->
                            reply.Reply(b)
                            return! loop() }
                loop())
    let fetch (agent:Agent<_,_>) =
        agent.PostAndReply(Fetch)
module Async =
    let map f a =
        async {
            let! a = a
            return f a }
let output = Agent.make()
let tokenizer =
    output
    |> Agent.map
        (fun s ->
            async {
                let! s = s
                return! s |> Seq.map (fun x -> async { return Tokenizer.tokenize x }) |> Async.Parallel })
let categorizer = tokenizer |> Agent.map (Async.map LineCategorizer.categorize)
let commentStripper = categorizer |> Agent.map (Async.map CommentStripper.stripComments)
let preprocessor = commentStripper |> Agent.map (Async.map (fun (a,b,c:string seq) -> Preprocessor.preprocess Preprocessor.realFileResolver a b c))
let start (f:FileInfo) =
    async {
        let directory = f.Directory.FullName |> Some
        let file = f.Name
        let lines = f.FullName |> File.ReadAllLines |> Seq.ofArray
        return (file, directory, lines) }
    |> NewData
    |> preprocessor.Post
start (FileInfo(@"D:\NodeJs\TextOn.Atom\examples\example.texton"))
async { return! output |> Agent.fetch |> Option.get } |> Async.RunSynchronously

