namespace TextOn.Atom

/// Unique identifier for an attribute in some cache we prepare.
type AttributeIdentity = int

/// Represents a tree of decisions.
type Condition =
    | True
    | Both of Condition * Condition
    | Either of Condition * Condition
    | AreEqual of AttributeIdentity * string
    | AreNotEqual of AttributeIdentity * string

/// Resolve a condition from a cache of values.
[<RequireQualifiedAccess>]
module ConditionEvaluator =
    /// Resolve a condition's value.
    let rec resolve (cache:Map<int, string>) condition : bool =
        match condition with
        | True -> true
        | Both(c1, c2) -> (c1 |> resolve cache) && (c2 |> resolve cache)
        | Either(c1, c2) -> (c1 |> resolve cache) || (c2 |> resolve cache)
        | AreEqual(identity, value) -> (cache |> Map.find identity) = value
        | AreNotEqual(identity, value) -> (cache |> Map.find identity) <> value

    /// Resolve a condition's value with only a partial cache.
    let rec resolvePartial (cache:Map<int, string>) condition : bool =
        match condition with
        | True -> true
        | Both(c1, c2) -> (c1 |> resolvePartial cache) && (c2 |> resolvePartial cache)
        | Either(c1, c2) -> (c1 |> resolvePartial cache) || (c2 |> resolvePartial cache)
        | AreEqual(identity, value) -> cache |> Map.tryFind identity |> Option.map ((=) value) |> defaultArg <| true
        | AreNotEqual(identity, value) -> cache |> Map.tryFind identity |> Option.map ((<>) value) |> defaultArg <| true
