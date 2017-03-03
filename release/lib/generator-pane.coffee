###
  Manages the Generator Pane.
###

{CompositeDisposable} = require 'atom'
GeneratorPaneTitle = 'TextOn Generator'
textOnCore = require './texton-core'

findOrOpenGeneratorPaneView = ->
  for pane in window.atom.workspace.getPanes()
    for item in pane.getItems()
      if item.getTitle() is GeneratorPaneTitle
        pane.activateItem item
        pane.activate()
        return item
  window.atom.workspace.open(GeneratorPaneTitle, {split: 'right'})

isGeneratorPane = (filePath) ->
  offset = filePath.length - GeneratorPaneTitle.length
  index = filePath.indexOf(GeneratorPaneTitle, offset)
  index is offset

generator = null

updateGeneratorPane = (data) ->
  findOrOpenGeneratorPaneView()
  generator.update data

sendToTextOnGenerator = ->
  editor = atom.workspace.getActiveTextEditor()
  isTextOn = textOnCore.isTextOnEditor editor
  if isTextOn && editor.buffer.file?
    line = editor.getCursorBufferPosition().row
    text = editor.getText()
    req =
      FileName : editor.getPath()
      LineNumber : line
      Lines : text.split ["\n"]
    p = textOnCore.send('generatorstart', 'generatorSetup', req)
    p.then (data) ->
      if data.length is 1
        updateGeneratorPane (data[0])

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.workspace.addOpener (filePath) =>
      @createGeneratorPane() if isGeneratorPane filePath
    @subscriptions.add atom.commands.add 'atom-text-editor', 'TextOn:Send-To-Generator', ->
      sendToTextOnGenerator()

  deactivate: ->
    @subscriptions.dispose()

  createGeneratorPane: ->
    GeneratorPaneView = require './generator-pane-view'
    generator = new GeneratorPaneView(
        {
          collapsedSections : []
          functionName : ''
          canGenerate : false
          output : []
          fileName : []
          attributes : []
          variables : []
        })
