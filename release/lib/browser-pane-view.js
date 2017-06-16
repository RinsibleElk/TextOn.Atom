/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import BrowserPaneTreeView from './browser-pane-tree-view'

export default class BrowserPaneView {
  constructor (props) {
    this.props = props;
    etch.initialize(this);
    this.handleEvents();
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

  handleClickEvent (e) {
    // This prevents accidental collapsing when a .entries element is the event target
    if (e.target.classList.contains('entries')) {
      return
    }
    if (!(e.shiftKey || e.metaKey || e.ctrlKey)) {
      this.entryClicked(e)
    }
  }

  handleEvents () {
    const handleClickEvent = this.handleClickEvent.bind(this);
    this.element.addEventListener('click', handleClickEvent);
  }

  entryClicked (e) {
    const entry = e.target.closest('.entry');
    //selectEntry(entry);
    if (entry.classList.contains('texton-tree')) {
      entry.toggleExpansion();
    }
  }

  render () {
    return (
      <div className='texton-browser tool-panel' tabIndex='-1'>
        <ol class='list-tree has-collapsable-children'>
          <BrowserPaneTreeView
            text='Foo'
            isCollapsed={false} />
          <BrowserPaneTreeView
            text='Bar'
            isCollapsed={true} />
          <BrowserPaneTreeView
            text='Baz'
            isCollapsed={false} />
        </ol>
      </div>
    )
  }
}
