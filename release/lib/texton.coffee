###
  Main.
###

{CompositeDisposable} = require 'atom'
textOnCore = require './texton-core'
generatorPane = require './generator-pane'
browserPanel = require './browser-panel'

gotoDefinition = ->
  editor = atom.workspace.getActiveTextEditor()
  isTextOn = textOnCore.isTextOnEditor editor
  if isTextOn && editor.buffer.file?
    row = editor.getCursorBufferPosition().row
    column = editor.getCursorBufferPosition().column
    line = editor.buffer.lines[row]
    req =
      FileName : editor.getPath()
      Line : line
      Column : column - 1
    p = textOnCore.send('navigatetosymbol', 'navigate', req)
    p.then (data) ->
      if data.length is 1
        textOnCore.navigate data[0]

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.commands.add 'atom-text-editor', 'TextOn:Go-To-Definition', ->
      gotoDefinition()
    @subscriptions.add atom.commands.add 'atom-workspace', 'TextOn:Settings', -> atom.workspace.open "atom://config/packages/texton"
    @subscriptions.add atom.contextMenu.add
      'atom-text-editor': [
        {
          label: 'TextOn'
          submenu: [
            {label: 'Send To Generator', command: 'TextOn:Send-To-Generator'},
            {label: 'Go To Definition', command: 'TextOn:Go-To-Definition'}
            {label: 'View Browser Panel', command: 'TextOn:View-Browser'}
          ]
          shouldDisplay: (event) => @shouldDisplayContextMenu(event)
        }
      ]
    generatorPane.activate()
    browserPanel.activate()
    textOnCore.spawn()
  deactivate: ->
    generatorPane.deactivate()
    browserPanel.deactivate()
    textOnCore.kill()
    @subscriptions.dispose()
  shouldDisplayContextMenu: (event) ->
    editor = atom.workspace.getActiveTextEditor()
    textOnCore.isTextOnEditor editor
  provideErrors: ->
    return require('./texton-linter')
  provideCompletions: ->
    return require('./texton-autocomplete')
  config:
    UseLinter:
      type: 'boolean'
      default: true
    DeveloperMode:
      type: 'boolean'
      default: false
    TextOnPath:
      type: 'string'
      default: ''
    MonoPath:
      type: 'string'
      default: '/usr/bin'
    GeneratorConfig:
      type: 'object'
      properties:
        NumSpacesBetweenSentences:
          title: 'Number of spaces between sentences'
          type: 'number'
          enum: [1,2]
          default: 2
        NumBlankLinesBetweenParagraphs:
          title: 'Number of blank lines between paragraphs'
          type: 'number'
          enum: [0,1]
          default: 1
        WindowsLineEndings:
          title: 'Use Windows line endings?'
          type: 'boolean'
          default: false
