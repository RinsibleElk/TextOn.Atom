/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import Logger from './texton-logger'

export default class BrowserPaneTreeView {
  constructor (props) {
    Logger.logf("BrowserPaneTreeView", "Ctor", props)
    this.props = props;
    this.isExpanded = false
    etch.initialize(this);
    this.element.classList.add('list-nested-item');
    this.element.classList.add('collapsed');
    this.element.classList.add('texton-tree');
    this.element.classList.add('entry');
    this.element.collapse = this.collapse.bind(this)
    this.element.expand = this.expand.bind(this)
    this.element.toggleExpansion = this.toggleExpansion.bind(this);
    if (!this.props.isCollapsed)
    {
      this.expand();
    }
  }

  expand () {
    Logger.logf("BrowserPaneTreeView", "Expand", [])
    TextOnCore.send('browserexpand', 'browseritems', { browserFile : this.props.browserFile, indexPath : this.props.indexPath })
      .then((data) => {
        if (data.length > 0) {
          this.items = data[0].items
          this.isExpanded = true;
          this.element.classList.add('expanded')
          this.element.classList.remove('collapsed')
        }
      })
  }

  collapse () {
    Logger.logf("BrowserPaneTreeView", "Collapse", [])
    this.isExpanded = false;
    this.element.classList.remove('expanded')
    this.element.classList.add('collapsed')
    this.items = []
  }

  toggleExpansion () {
    Logger.logf("BrowserPaneTreeView", "toggleExpansion", [])
    if (this.isExpanded) {
      this.collapse();
    } else {
      this.expand();
    }
  }

  destroy () {
  }

  update (props) {
    if (props.hasOwnProperty('text')) {
      this.props.text = props.text;
    }
    if (props.hasOwnProperty('file')) {
      this.props.file = props.file;
    }
    if (props.hasOwnProperty('line')) {
      this.props.line = props.line;
    }
    if (props.hasOwnProperty('isCollapsed')) {
      this.props.isCollapsed = props.isCollapsed;
    }
    if (props.hasOwnProperty('items')) {
      this.props.items = props.items
      shouldComputeItems = true
    }
    if (props.hasOwnProperty('indexPath')) {
      this.props.indexPath = props.indexPath
    }
    if (props.hasOwnProperty('browserFile')) {
      this.props.browserFile = props.browserFile
    }
    if (shouldComputeItems) {
      this.computeItems()
    }
    return etch.update(this)
  }

  computeItems () {
    this.items = this.props.items;
  }

  didClickLink () {
    data =
      {
        FileName : this.props.file,
        LineNumber : this.props.line,
        Location : 1
      }
    TextOnCore.navigate(data)
    return false;
  }

  render () {
    return (
      <li>
        <div class='list-item'>
          <a class='entry' onClick={this.didClickLink.bind(this)}>{this.props.text}</a>
        </div>
        <ol class='list-tree entry'>
          <li class='list-item'>
            <span>{this.props.text}</span>
          </li>
        </ol>
      </li>
    )
  }
}
