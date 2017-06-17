/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import Logger from './texton-logger'

export default class BrowserPaneTreeView {
  constructor (props) {
    this.props = props;
    this.items = this.props.items;
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
          this.isExpanded = true;
          this.element.classList.add('expanded')
          this.element.classList.remove('collapsed')
          this.update({ items : data[0].newItems })
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
    shouldComputeItems = false
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
    Logger.logf("Computing Items", "Blah", [this.items])
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

  renderItems () {
    if (this.items.length > 0) {
      const className = 'list-tree has-collapsable-children';
      Logger.logf("Well", "We Made it This Far", [this.items])
      return $.ol(
        {className, ref: 'items'},
        ...this.items.map((item, index) => $(BrowserPaneTreeView, {
          text : item.text,
          file : item.file,
          line : item.line,
          isCollapsed : item.isCollapsed,
          browserFile : this.props.file,
          indexPath : item.indexPath,
          items : item.children
        }))
      )
    } else {
      return ""
    }
  }

  render () {
    return (
      <li>
        <div class='list-item'>
          <a class='entry' onClick={this.didClickLink.bind(this)}>{this.props.text}</a>
        </div>
        {this.renderItems()}
      </li>
    )
  }
}
