path = require 'path'

child_process = require('child_process')

port = ((Math.random() * (8999.0 - 8100.0)) + 8100.0).toString().substring(0, 4)

child = null

isWin = () ->
  window.process.platform.indexOf("win") is 0

spawnWin = (location) ->
  location = location + "\\TextOn.Atom.exe"
  try
    child = child_process.spawn(location, [ "--port", port ])
  catch e
    child = null

spawnMono = (location, monoPath) ->
  location = location + "/TextOn.Atom.exe"
  mono = monoPath + "/mono"
  try
    child = child_process.spawn(mono, [ location, "--port", port ])
  catch
    child = null

module.exports =
  spawn: ->
    textOnPath = window.atom.config.get("texton.TextOnPath")
    if isWin() then spawnWin(textOnPath)
    else spawnMono(textOnPath, window.atom.config.get("texton.MonoPath"))
  kill: ->
    child?.kill("SIGKILL")
  isTextOnEditor: (editor) ->
    editor?.getGrammar().name.indexOf("texton") >= 0
  port: ->
    port
