###
  Implements API for linter.
###

# Should this just be about errors?
linter = window.atom.config.get("texton.UseLinter")
textOnCore = require './texton-core'
path = require 'path'

module.exports = new class # This only needs to be a class to bind lint()
  grammarScopes: ['source.texton']
  scope: "file"
  lintOnFly: true
  lint: (textEditor) =>
    return [] unless linter
    isTextOn = textOnCore.isTextOnEditor textEditor
    return [] unless isTextOn
    fileName = textEditor.getPath()
    text = textEditor.getText()
    data =
      FileName: fileName
      Lines: text.split(["\n"])
    # Technically I should probably receive lint as well here?
    textOnCore.send("parse", "errors", data)
