###
  Manages the Browser Pane.
###

{CompositeDisposable} = require 'atom'
textOnCore = require './texton-core'
Logger = require './texton-logger'
browser = null

createBrowserPane = ->
  BrowserPaneView = require './browser-pane-view'
  browser = new BrowserPaneView(
      {
        name : 'World'
      })

showBrowser = ->
  paneContainer = atom.workspace.paneContainerForURI(browser.getURI())
  if paneContainer?
    paneContainer.show()
    pane = atom.workspace.paneForItem(browser)
    if pane?
      pane.activateItemForURI(browser.getURI())

findOrOpenBrowserPaneView = ->
  if browser?
      showBrowser()
  else
    createBrowserPane()
    atom.workspace.open(browser, {
      activatePane: false,
      activateItem: false
    }).then () ->
      showBrowser()

updateBrowserPane = ->
  findOrOpenBrowserPaneView()
  browser.update({name:'Updated'})

sendToTextOnBrowser = ->
  updateBrowserPane()

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
