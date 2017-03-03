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

valueset = (type, name, value) ->
  req =
    Type : type
    Name : name
    Value : value
  p = textOnCore.send('generatorvalueset', 'generatorSetup', req)
  p.then (data) ->
    if data.length > 0
      updateGeneratorPane (data[0])

navigate = (type, fileName, name) ->
  req =
    FileName : fileName
    NavigateType : type
    Name : name
  p = textOnCore.send('navigaterequest', 'navigate', req)
  p.then (data) ->
    if data.length is 1
      textOnCore.navigate data[0]

requestUpdate = ->
  req =
    Blank : ""
  p = textOnCore.send('updategenerator', 'generatorSetup', req)
  p.then (data) ->
    if data.length > 0
      updateGeneratorPane (data[0])

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.workspace.addOpener (filePath) =>
      @createGeneratorPane() if isGeneratorPane filePath
    @subscriptions.add atom.commands.add 'atom-text-editor', 'TextOn:Send-To-Generator', ->
      sendToTextOnGenerator()
    @disp = null
    @subscriptions.add atom.workspace.onDidStopChangingActivePaneItem (item) ->
      @disp?.dispose()
      @disp = null
      if generator?
        if atom.workspace.isTextEditor item
          if textOnCore.isTextOnEditor item
            @disp = item.onDidStopChanging ->
              requestUpdate()
    @subscriptions.add atom.workspace.onDidDestroyPaneItem (event) =>
      item = event.item
      if item is generator
        @disp?.dispose()
        @disp = null
        generator = null
      return

  deactivate: ->
    @disp?.dispose()
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
          onDidClickSmartLink : (type, fileName, name) -> navigate(type, fileName, name)
          onDidConfirmSelection : (type, name, value) -> valueset(type, name, value)
        })
