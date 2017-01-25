#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

type Message<'a,'b> =
    | Quit
    | NewData of 'a
    | Fetch of 'b option AsyncReplyChannel
type Agent<'a,'b> = Message<'a,'b> MailboxProcessor
[<RequireQualifiedAccess>]
module Agent =
    let map (f:'a -> 'b) (next:Agent<'b,'c>) =
        Agent.Start
            (fun inbox ->
                let mutable b = None
                let rec loop() =
                    async {
                        let! msg = inbox.Receive()
                        match msg with
                        | Quit -> return ()
                        | NewData a ->
                            b <- Some (f a)
                            next.Post(NewData(b.Value))
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
                            printfn "%A" a
                            return! loop()
                        | Fetch(reply) ->
                            reply.Reply(b)
                            return! loop() }
                loop())

let output = Agent.make()
let tokenizer = output |> Agent.map (Seq.map Tokenizer.tokenize)
let categorizer = tokenizer |> Agent.map LineCategorizer.categorize
let commentStripper = categorizer |> Agent.map CommentStripper.stripComments
let preprocessor = commentStripper |> Agent.map (fun (a,b,c:string seq) -> Preprocessor.preprocess Preprocessor.realFileResolver a b c)
let start (f:FileInfo) =
    let directory = f.Directory.FullName |> Some
    let file = f.Name
    let lines = f.FullName |> File.ReadAllLines |> Seq.ofArray
    preprocessor.Post(NewData(file, directory, lines))
start (FileInfo(@"D:\NodeJs\TextOn.Atom\examples\example.texton"))

