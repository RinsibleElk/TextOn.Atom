###
  Logging component for developer mode.
###

debug = window.atom.config.get("texton.DeveloperMode")
module.exports =
  logf: (category, format, data) ->
    if debug
      msg = "[#{category}] #{format}"
      if data.length is 0 then console.log(msg)
      else console.log.apply(console, [msg].concat(data))
