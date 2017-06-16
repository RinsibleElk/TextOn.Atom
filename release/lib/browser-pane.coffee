###
  Manages the Browser Pane.
###

{CompositeDisposable} = require 'atom'
textOnCore = require './texton-core'
Logger = require './texton-logger'
browser = null

createBrowserPane = (data) ->
  BrowserPaneView = require './browser-pane-view'
  browser = new BrowserPaneView(
      {
        nodes : data.nodes
      })

showBrowser = ->
  paneContainer = atom.workspace.paneContainerForURI(browser.getURI())
  if paneContainer?
    paneContainer.show()
    pane = atom.workspace.paneForItem(browser)
    if pane?
      pane.activateItemForURI(browser.getURI())

updateBrowserPane = (data) ->
  if browser?
      browser.update data
      showBrowser()
  else
    createBrowserPane data
    Logger.logf("updateBrowserPane", "finished creating", browser)
    atom.workspace.open(browser, {
      activatePane: false,
      activateItem: false
    }).then () ->
      showBrowser()

sendToTextOnBrowser = ->
  editor = atom.workspace.getActiveTextEditor()
  isTextOn = textOnCore.isTextOnEditor editor
  if isTextOn && editor.buffer.file?
    text = editor.getText()
    req =
      FileName : editor.getPath()
      Lines : text.split ["\n"]
    p = textOnCore.send('browserstart', 'browserUpdate', req)
    p.then (data) ->
      if data.length is 1
        Logger.logf("sendToTextOnBrowser", "updateBrowserPane", data[0])
        updateBrowserPane (data[0])

requestUpdate = ->

module.exports =
  activate: ->
    @subscriptions = new CompositeDisposable
    @subscriptions.add atom.commands.add 'atom-text-editor', 'TextOn:View-Browser', ->
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