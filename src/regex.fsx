open System.Text.RegularExpressions
let s =
  "var $Country =\n  \"Which country are you writing about?\",\n  [\n    *\n  ]";
let re =
  new Regex("^(var)\\s+\\$[A-Za-z][A-Za-z0-9_]*\\s*=\\s*(\".*\")\\s*,\\s*(\\[\\s*\\*\\s*\\])")
re.Match(s)
