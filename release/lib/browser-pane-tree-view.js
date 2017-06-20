/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import Logger from './texton-logger'

export default class BrowserPaneTreeView {
  constructor (props) {
    this.props = props;
    this.children = [];
    this.items = this.props.items;
    this.isExpanded = false
    this.isNested = true
    this.parentNode = props.parentNode;
    etch.initialize(this);
    this.element.classList.add('list-nested-item');
    this.element.classList.add('collapsed');
    this.element.classList.add('texton-tree');
    this.element.classList.add('entry');
    this.element.browserNode = this;
    this.element.collapse = this.collapse.bind(this)
    this.element.expand = this.expand.bind(this)
    this.element.toggleExpansion = this.toggleExpansion.bind(this);
    if (props.onDidInitialize) {
      props.onDidInitialize(this)
    }
    this.computeNesting()
    if (!this.props.isCollapsed)
    {
      this.setExpanded();
    } else {
      this.setCollapsed();
    }
  }

  expand () {
    TextOnCore.send('browserexpand', 'browseritems', { browserFile : this.props.browserFile, indexPath : this.props.indexPath })
      .then((data) => {
        if (data.length > 0) {
          this.update({ items : data[0].newItems, isCollapsed : false })
        }
      })
  }

  setExpanded () {
    this.isExpanded = true;
    this.element.classList.add('expanded')
    this.element.classList.remove('collapsed')
  }

  collapse () {
    TextOnCore.send('browsercollapse', 'thanks', { browserFile : this.props.browserFile, indexPath : this.props.indexPath })
      .then((data) => {
        this.update({ items : [], isCollapsed : true })
      })
  }

  setCollapsed () {
    this.isExpanded = false;
    this.element.classList.remove('expanded')
    this.element.classList.add('collapsed')
  }

  toggleExpansion () {
    if (this.isExpanded) {
      this.collapse();
    } else {
      this.expand();
    }
  }

  destroy () {
    if (this.children != null) {
      for (const child of this.children) {
        child.destroy();
      }
      this.children = null;
    }
  }

  update (props) {
    shouldComputeItems = false
    if (props.hasOwnProperty('text')) {
      this.props.text = props.text;
    }
    if (props.hasOwnProperty('nodeType')) {
      this.props.nodeType = props.nodeType;
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
    if (props.hasOwnProperty('isCollapsible')) {
      this.props.isCollapsible = props.isCollapsible;
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
    if (props.hasOwnProperty('parentNode')) {
      this.props.parentNode = props.parentNode;
      this.parentNode = props.parentNode;
    }
    if (shouldComputeItems) {
      this.computeItems()
    }
    this.computeNesting()
    if (this.props.isCollapsible) {
      this.element.classList.add('texton-collapsible');
    } else {
      this.element.classList.remove('texton-collapsible');
    }
    if (!this.props.isCollapsed)
    {
      this.setExpanded();
    } else {
      this.setCollapsed();
    }
    return etch.update(this)
  }

  computeItems () {
    this.items = this.props.items;
  }

  computeNesting () {
    if (this.props.isCollapsible) {
      if (!this.isNested) {
        this.isNested = true
        this.element.classList.remove('list-item')
        this.element.classList.add('list-nested-item')
      }
    } else {
      if (this.isNested) {
        this.isNested = false
        this.element.classList.remove('list-nested-item')
        this.element.classList.add('list-item')
      }
    }
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

  didInitializeChild (child) {
    this.children.push(child);
  }

  renderItems () {
    if (this.items.length > 0) {
      className = 'list-tree has-collapsable-children';
      return $.ol(
        {className, ref: 'items'},
        ...this.items.map((item, index) => $(BrowserPaneTreeView, {
          text : item.text,
          nodeType : item.nodeType,
          file : item.file,
          line : item.line,
          isCollapsed : item.isCollapsed,
          isCollapsible : item.isCollapsible,
          browserFile : this.props.browserFile,
          indexPath : item.indexPath,
          items : item.children,
          parentNode : this,
          onDidInitialize: this.didInitializeChild.bind(this)
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
          <a onClick={this.didClickLink.bind(this)}>{this.props.text}</a>
        </div>
        {this.renderItems()}
      </li>
    )
  }
}
