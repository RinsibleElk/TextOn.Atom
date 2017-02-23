{$, $$$, ScrollView} = require 'atom-space-pen-views'

module.exports =
class GeneratorPane extends ScrollView
  @content: ->
    @div class: 'native-key-bindings', =>
      @div class: 'block pull-right', =>
        @button class: 'btn btn-sm', click: 'quit', 'Quit'

  constructor: (@editorId) ->
    super

  getTitle: ->
      "TextOn Generator"

  attached: ->
    @html $$$ ->
      @div class: 'texton'
