textOnCore = require './texton-core'
Logger = require './texton-logger'
path = require 'path'
rp = require 'request-promise'

module.exports = new class # This only needs to be a class to bind lint()
  grammarScopes: ['source.texton']
  scope: "file"
  lintOnFly: true
  lint: (textEditor) =>
    linter = window.atom.config.get("texton.UseLinter")
    return [] unless linter
    isTextOn = textOnCore.isTextOnEditor textEditor
    return [] unless isTextOn
    port = textOnCore.port()
    fileName = textEditor.getPath()
    text = textEditor.getText()
    data =
      FileName: fileName
      Lines: text.split(["\n"])
    Logger.logf("Service", "Sending parse request", [data])
    options =
      method: 'POST'
      uri: "http://localhost:#{port}/parse"
      json: true
      body: data
    rp(options).then (response) ->
      result = JSON.parse response
      Logger.logf("Service", "Received response", [result])
      return result.Data unless result.Kind isnt "errors"
      []
