###
  Manages the Generator Pane.
###

{CompositeDisposable} = require 'atom'
GeneratorPaneTitle = 'TextOn Generator'

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

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.workspace.addOpener (filePath) =>
      @createGeneratorPane() if isGeneratorPane filePath
    @subscriptions.add atom.commands.add 'atom-workspace', 'TextOn:Show-Generator', ->
      findOrOpenGeneratorPaneView()

  deactivate: ->
    @subscriptions.dispose()

  createGeneratorPane: ->
    GeneratorPaneView = require './generator-pane-view'
    new GeneratorPaneView({ collapsedSections : [] })
