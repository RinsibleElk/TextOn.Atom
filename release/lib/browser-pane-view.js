/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import PaneSectionView from './pane-section-view'
import BrowserPaneTreeView from './browser-pane-tree-view'

export default class BrowserPaneView {
  constructor (props) {
    this.props = props;
    this.collapsedSections = props.collapsedSections ? new Set(props.collapsedSections) : new Set();
    this.inputs = [];
    this.sections = [];
    this.attributes = [];
    this.variables = [];
    etch.initialize(this);
    for (const section of this.sections) {
      if (this.collapsedSections.has(section.name)) {
        section.collapse();
      } else {
        section.expand();
      }
    }
    this.handleEvents();
  }

  destroy () {
  }

  update (props) {
    if (props.hasOwnProperty('nodes')) {
      this.props.nodes = props.nodes
    }
    if (props.hasOwnProperty('file')) {
      this.props.file = props.file
    }
    return etch.update(this)
  }

  renderItems () {
    if (this.props.nodes.length > 0) {
      const className = 'list-tree has-collapsable-children';
      return $.ol(
        {className, ref: 'items'},
        ...this.props.nodes.map((item, index) => $(BrowserPaneTreeView, {
          text : item.text,
          nodeType : item.nodeType,
          file : item.file,
          line : item.line,
          isCollapsed : item.isCollapsed,
          isCollapsible : item.isCollapsible,
          browserFile : this.props.file,
          indexPath : item.indexPath,
          items : []
        }))
      )
    } else {
      return ""
    }
  }

  didInitializeSection (section) {
    this.sections.push(section);
  }

  didInitializeInput (input) {
    this.inputs.push(input);
  }

  didConfirmSelection (type, name, value) {
    this.props.onDidConfirmSelection (type, name, value)
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
    if (entry != null) {
      if (entry.classList.contains('texton-tree')) {
        entry.toggleExpansion();
      }
    }
  }

  render () {
    return (
      <div className='texton-browser tool-panel' tabIndex='-1'>
        <main className='texton-sections'>
          <PaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='browser' title='Browser'>
            {this.renderItems()}
          </PaneSectionView>
        </main>
      </div>
    )
  }
}
