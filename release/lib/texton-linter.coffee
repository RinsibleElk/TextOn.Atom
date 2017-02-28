textOnCore = require './texton-core'
Logger = require './texton-logger'
path = require 'path'
{$} = require 'atom-space-pen-views'

module.exports = new class # This only needs to be a class to bind lint()
  grammarScopes: ['source.texton']
  scope: "file"
  lintOnFly: true
  lint: (textEditor) ->
    Logger.logf("Debug", "Called lint", [])
    linter = window.atom.config.get("texton.UseLinter")
    return [] unless linter
    isTextOn = textOnCore.isTextOnEditor textEditor
    return [] unless isTextOn
    port = textOnCore.port()
    fileName = textEditor.getPath()
    text = textEditor.getText()
    data =
      FileName: fileName
      IsAsync: true
      Lines: text.split(["\n"])
    Logger.logf("Service", "Sending parse request", [fileName, data])
    p = $.post
          url: "http://localhost:#{port}/parse"
          dataType: 'json'
          data: data
    p.done()
