{$, $$$, ScrollView} = require 'atom-space-pen-views'

module.exports =
class GeneratorPane extends ScrollView
  @content: ->
    @div class: 'native-key-bindings', tabindex: -1

  constructor: (@editorId) ->
    super

  getTitle: ->
      "TextOn Generator"

  attached: ->
    @html $$$ ->
      @div class: 'texton'
