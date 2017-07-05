###
  Logging component for developer mode.
###

debug = window.atom.config.get("texton.DeveloperMode")
module.exports =
  logf: (category, format, data) ->
    if debug
      today = new Date
      time = '' + today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds() + "." + today.getMilliseconds()
      msg = "#{time} [#{category}] #{format}"
      if data.length is 0 then console.log(msg)
      else console.log.apply(console, [msg].concat(data))
