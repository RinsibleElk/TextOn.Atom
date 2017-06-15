/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'

export default class BrowserPaneView {
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
    return other instanceof BrowserPaneView;
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

  getTitle () {
    return "TextOn Browser";
  }

  getURI () {
    return "atom://texton-browser";
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
