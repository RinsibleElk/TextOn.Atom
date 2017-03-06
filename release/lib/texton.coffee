###
  Main.
###

textOnCore = require './texton-core'
generatorPane = require './generator-pane'

module.exports =
  activate: ->
    window.atom.commands.add("atom-workspace", "TextOn:Settings", -> window.atom.workspace.open "atom://config/packages/texton")
    generatorPane.activate()
    textOnCore.spawn()
  deactivate: ->
    generatorPane.deactivate()
    textOnCore.kill()
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
