###
  Core component for messaging to the TextOn process.
###

path = require 'path'

child_process = require('child_process')
Logger = require './texton-logger'
rp = require 'request-promise'

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
  navigate: (data) ->
    fileName = data.FileName
    lineNumber = data.LineNumber - 1
    location = data.Location - 1
    for pane in window.atom.workspace.getPanes()
      for item in pane.getItems()
        if item.getPath?() is fileName
          pane.activate()
          pane.activateItem(item)
          item.setCursorBufferPosition([lineNumber, location])
          return
    window.atom.workspace.open(fileName,
      initialLine : lineNumber
      initialColumn : location)
  generatorConfig: ->
    o =
      NumSpacesBetweenSentences: window.atom.config.get("texton.GeneratorConfig.NumSpacesBetweenSentences")
      NumBlankLinesBetweenParagraphs: window.atom.config.get("texton.GeneratorConfig.NumBlankLinesBetweenParagraphs")
      WindowsLineEndings: window.atom.config.get("texton.GeneratorConfig.WindowsLineEndings")
    o
  send: (requestKind, responseKind, data) ->
    Logger.logf("Service", "Sending #{requestKind} request", [data])
    options =
      method: 'POST'
      uri: "http://localhost:#{port}/#{requestKind}"
      json: true
      body: data
    rp(options).then (response) ->
      result = JSON.parse response
      Logger.logf("Service", "Received response", result)
      return result.Data unless result.Kind isnt responseKind
      []
