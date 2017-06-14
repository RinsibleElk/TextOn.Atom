###
  Manages the Browser Panel.
###

{CompositeDisposable} = require 'atom'
BrowserPanelTitle = 'TextOn Browser'
textOnCore = require './texton-core'

findOrOpenBrowserPanelView = ->
  for pane in window.atom.workspace.getPanes()
    for item in pane.getItems()
      if item.getTitle() is BrowserPanelTitle
        pane.activateItem item
        pane.activate()
        return item
  window.atom.workspace.open(BrowserPanelTitle, {split: 'bottom'})

isBrowserPanel = (filePath) ->
  offset = filePath.length - BrowserPanelTitle.length
  index = filePath.indexOf(BrowserPanelTitle, offset)
  index is offset

browser = null

updateBrowserPanel = ->
  findOrOpenBrowserPanelView()
  browser.update({name:'Updated'})

sendToTextOnBrowser = ->
  updateBrowserPanel()

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.workspace.addOpener (filePath) =>
      @createBrowserPanel() if isBrowserPanel filePath
    @subscriptions.add atom.commands.add 'atom-text-editor', 'TextOn:Send-To-Browser', ->
      sendToTextOnBrowser()
    @disp = null
    @subscriptions.add atom.workspace.onDidStopChangingActivePaneItem (item) ->
      @disp?.dispose()
      @disp = null
      if browser?
        if atom.workspace.isTextEditor item
          if textOnCore.isTextOnEditor item
            @disp = item.onDidStopChanging ->
              requestUpdate()
    @subscriptions.add atom.workspace.onDidDestroyPaneItem (event) =>
      item = event.item
      if item is browser
        @disp?.dispose()
        @disp = null
        browser = null
      return

  deactivate: ->
    @disp?.dispose()
    @subscriptions.dispose()

  createBrowserPanel: ->
    BrowserPanelView = require './browser-panel-view'
    browser = new BrowserPanelView(
        {
          name : 'World'
        })
