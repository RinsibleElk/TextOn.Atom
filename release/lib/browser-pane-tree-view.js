/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'

export default class BrowserPaneTreeView {
  constructor (props) {
    this.props = props;
    etch.initialize(this);
    this.element.classList.add('list-nested-item');
    this.element.collapse = this.collapse.bind(this)
    this.element.expand = this.expand.bind(this)
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
        <span>{this.props.text}</span>
      </li>
    )
  }
}
