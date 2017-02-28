{ScrollView} = require 'atom-space-pen-views'

module.exports =
class GeneratorPaneView extends ScrollView
  @content: ->
    @div class: 'texton-generator-pane-view pane-item', tabIndex: -1, =>
      @h1 'TextOn Generator'

  getTitle: ->
    "TextOn Generator"
