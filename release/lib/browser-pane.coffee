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
        attributes : data.attributes,
        variables : data.variables,
        nodes : data.nodes,
        file : data.file,
        onDidClickSmartLink : (type, fileName, name) -> navigate(type, fileName, name),
        onDidConfirmSelection : (file, type, name, value) -> valueset(file, type, name, value)
      })

showBrowser = ->
  paneContainer = atom.workspace.paneContainerForURI(browser.getURI())
  if paneContainer?
    paneContainer.show()
    pane = atom.workspace.paneForItem(browser)
    if pane?
      pane.activateItemForURI(browser.getURI())

valueset = (file, type, name, value) ->
  req =
    FileName : file
    Type : type
    Name : name
    Value : value
  p = textOnCore.send('browservalueset', 'browserUpdate', req)
  p.then (data) ->
    if data.length > 0
      updateBrowserPane (data[0])

navigate = (type, fileName, name) ->
  req =
    FileName : fileName
    NavigateType : type
    Name : name
  p = textOnCore.send('navigaterequest', 'navigate', req)
  p.then (data) ->
    if data.length is 1
      textOnCore.navigate data[0]

updateBrowserPane = (data) ->
  if browser?
      browser.update data
      showBrowser()
  else
    createBrowserPane data
    atom.workspace.open(browser, {
      activatePane: false,
      activateItem: false
    }).then () ->
      showBrowser()
      browser.update data

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
