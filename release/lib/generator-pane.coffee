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
          collapsedSections : [],
          attributes : [
#            {
#              name : 'attOne'
#              text : 'Some info about this attribute.'
#              value : 'four'
#              items : ['one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight','nine','ten','eleven']
#            },
#            {
#              name : 'attTwo'
#              text : 'Some more info.'
#              value : ''
#              items : ['eight','nine','ten','eleven']
#            }
          ]
          variables : [
#            {
#              name : 'varOne'
#              text : 'This one does not permit free value. Start value: three.'
#              value : 'three'
#              permitsFreeValue: false
#              items : ['one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight','nine','ten','eleven']
#            },
#            {
#              name : 'varTwo'
#              text : 'This one does permit free value. Start value: eighteen.'
#              value : 'eighteen'
#              permitsFreeValue : true
#              items : ['eight','nine','ten','eleven']
#            },
#            {
#              name : 'varThree'
#              text : 'This one permits free value and has no suggestions. Start value: Elk.'
#              value : 'Elk'
#              permitsFreeValue : true
#              items : []
#            }
          ]
        })
