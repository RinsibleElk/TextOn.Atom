###
  Implements API for autocomplete-plus.
###

textOnCore = require './texton-core'
path = require 'path'
fuzzaldrin = require 'fuzzaldrin'

mapResult = (result) ->
  if result.type is 'variable'
    c = '$'
  else if result.type is 'attribute'
    c = '%'
  else
    c = '@'
  data =
    text: result.text
    type: result.type
    displayText: c + result.text
    description: result.description
    iconHTML: "<img height='16px' width='16px' src='atom://texton/styles/icons/autocomplete_#{result.type}@3x.png' />"

module.exports = new class # This only needs to be a class to bind getSuggestions()
  selector: '.source.texton'
  suggestionPriority: 2
  getSuggestions: (options) =>
    textEditor = options.editor
    isTextOn = textOnCore.isTextOnEditor textEditor
    return [] unless isTextOn
    row = options.bufferPosition.row
    col = options.bufferPosition.column
    line = options.editor.buffer.lines[row]
    prefix = options.prefix
    c = line.charAt(col - options.prefix.length - 1)
    if c is '@'
      data =
        fileName: textEditor.getPath()
        type: 'Function'
      p = textOnCore.send("autocomplete", "suggestion", data)
    else if c is '%'
      data =
        fileName: textEditor.getPath()
        type: 'Attribute'
      p = textOnCore.send("autocomplete", "suggestion", data)
    else if c is '$'
      data =
        fileName: textEditor.getPath()
        type: 'Variable'
      p = textOnCore.send("autocomplete", "suggestion", data)
    else
      return []
    p.then((res) =>
      scoredItems = []
      for item in res
        score = fuzzaldrin.score(item.text, prefix)
        if score > 0
          scoredItems.push({item, score})
      scoredItems.sort((a, b) => b.score - a.score)
      scoredItems.map((res) => mapResult(res.item)))
