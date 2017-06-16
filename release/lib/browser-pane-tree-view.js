/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import Logger from './texton-logger'

export default class BrowserPaneTreeView {
  constructor (props) {
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
    this.isExpanded = true;
    this.element.classList.add('expanded')
    this.element.classList.remove('collapsed')

  }

  collapse () {
    Logger.logf("BrowserPaneTreeView", "Collapse", [])
    this.isExpanded = false;
    this.element.classList.remove('expanded')
    this.element.classList.add('collapsed')
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
    if (props.hasOwnProperty('index')) {
      this.props.index = props.index;
    }
    if (props.hasOwnProperty('isCollapsed')) {
      this.props.isCollapsed = props.isCollapsed;
    }
    if (props.hasOwnProperty('children')) {
      this.props.children = props.children;
    }
    return etch.update(this)
  }

  render () {
    return (
      <li>
        <div class='list-item'>
          <span>{this.props.text}</span>
        </div>
        <ol class='list-tree'>
          <li class='list-item'>
            <span>{this.props.text}</span>
          </li>
        </ol>
      </li>
    )
  }
}
