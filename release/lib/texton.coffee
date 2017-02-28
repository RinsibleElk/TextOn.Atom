textOnCore = require './texton-core.coffee'

module.exports =
  activate: ->
    window.atom.commands.add("atom-workspace", "TextOn:Settings", -> window.atom.workspace.open "atom://config/packages/texton")
    textOnCore.spawn()
  deactivate: ->
    textOnCore.kill()
  provideErrors: ->
    return require('./texton-linter.coffee')
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
          type: 'number'
          default: 2
        NumBlankLinesBetweenParagraphs:
          type: 'number'
          default: 1
        WindowsLineEndings:
          type: 'boolean'
          default: false
