###
  Manages the Generator Pane.
###

{CompositeDisposable} = require 'atom'
GeneratorPaneTitle = 'TextOn Generator'
textOnCore = require './texton-core'
Logger = require './texton-logger'
generator = null
dock = 'right'

createGeneratorPane = ->
  GeneratorPaneView = require './generator-pane-view'
  Logger.logf("createGeneratorPane", "dock", [dock])
  generator = new GeneratorPaneView(
      {
        collapsedSections : []
        functionName : ''
        canGenerate : false
        output : []
        fileName : []
        attributes : []
        variables : []
        dock : dock
        onDidClickSmartLink : (type, fileName, name) -> navigate(type, fileName, name)
        onDidConfirmSelection : (type, name, value) -> valueset(type, name, value)
        onDidClickGenerate : -> generate()
        onDidClickSimpleLink : (item) ->
          data =
            FileName : item.File
            LineNumber : item.LineNumber
            Location : 1
          textOnCore.navigate(data)
      })

showGenerator = ->
  paneContainer = atom.workspace.paneContainerForURI(generator.getURI())
  if paneContainer?
    paneContainer.show()
    pane = atom.workspace.paneForItem(generator)
    if pane?
      pane.activateItemForURI(generator.getURI())

findOrOpenGeneratorPaneView = ->
  if generator?
    showGenerator()
  else
    createGeneratorPane()
    atom.workspace.open(generator, {
      activatePane: false,
      activateItem: false
    }).then () ->
      showGenerator()

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
      # We take care _not_ to open the generator here - that would be annoying.
      generator.update (data[0])

generate = ->
  req =
    Config : textOnCore.generatorConfig()
  p = textOnCore.send('generate', 'generatorSetup', req)
  p.then (data) ->
    if data.length > 0
      updateGeneratorPane (data[0])

module.exports =
  activate: ->
    Logger.logf("GeneratorPane", "Activate", [])
    dock = textOnCore.generatorDock()
    @subscriptions = new CompositeDisposable
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
    Logger.logf("GeneratorPane", "ActivateDone", [])

  deactivate: ->
    @disp?.dispose()
    @subscriptions.dispose()
