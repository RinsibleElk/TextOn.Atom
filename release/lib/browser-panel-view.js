/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'

export default class BrowserPanelView {
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

  getTitle () {
    return 'TextOn Browser';
  }

  isEqual (other) {
    return other instanceof BrowserPanelView;
  }

  getPreferredLocation () {
    return 'bottom';
  }

  getAllowedLocations () {
    return ["bottom"]
  }

  isPermanentDockItem () {
    return true;
  }

  render () {
    return (
      <div className='texton-browser tool-panel' tabIndex='-1'>
        <header className='texton-header'>
          <h1>TextOn Browser</h1>
        </header>
        <main>
          <div>Hello {this.props.name}!</div>
        </main>
      </div>
    )
  }
}
