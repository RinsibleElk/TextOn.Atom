namespace TextOn.Atom

open Newtonsoft.Json

[<RequireQualifiedAccess>]
module internal JsonSerializer =
    let writeJson(o: obj) = JsonConvert.SerializeObject(o, [||])
    let readJson<'t>(s: string) = JsonConvert.DeserializeObject<'t>(s)
