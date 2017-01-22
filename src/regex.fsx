open System.Text.RegularExpressions
let s =
  "Something or other {choice 1|choice 2}";
let re =
  new Regex("(\\{).*(\\})")
let m = re.Match(s)
m.Success |> printfn "%A"
m.Groups
|> fun a ->
    [ 0 .. (a.Count - 1) ]
    |> Seq.map (fun i -> a.[i])
|> Seq.iter
    (fun cap ->
        cap |> printfn "%A")


