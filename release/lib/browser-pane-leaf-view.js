/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'

export default class BrowserPaneLeafView {
  constructor (props) {
    this.props = props;
    etch.initialize(this);
  }

  destroy () {
  }

  update (props) {
    if (props.hasOwnProperty('name')) {
      this.props.name = props.name
    }
    return etch.update(this)
  }

  render () {
    return (
      <div>
      </div>
    )
  }
}
