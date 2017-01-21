open System.Text.RegularExpressions
let s =
  "var $Country =  \"Which country are you writing about?\",  [    *  ]";
let re =
  new Regex("^(var)\\s+\\$[A-Za-z][A-Za-z0-9_]*")//"\\s*=\\S*(\".*\")\\s*,\\S*(\\[.*\\])")
re.Match(s)
