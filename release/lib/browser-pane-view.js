/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import TextOnCore from './texton-core'
import PaneSectionView from './pane-section-view'
import ValueInputView from './value-input-view'
import BrowserPaneTreeView from './browser-pane-tree-view'
import Logger from './texton-logger'

export default class BrowserPaneView {
  constructor (props) {
    this.props = props;
    this.collapsedSections = props.collapsedSections ? new Set(props.collapsedSections) : new Set();
    this.inputs = [];
    this.sections = [];
    this.children = [];
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
    for (const input of this.inputs) {
      input.destroy();
    }
    for (const section of this.sections) {
      section.destroy();
    }
    for (const child of this.children) {
      children.destroy();
    }
    this.inputs = null;
    this.sections = null;
    this.children = null;
  }

  update (props) {
    if (props.hasOwnProperty('attributes')) {
      this.props.attributes = props.attributes
    }
    if (props.hasOwnProperty('variables')) {
      this.props.variables = props.variables
    }
    if (props.hasOwnProperty('nodes')) {
      this.props.nodes = props.nodes
    }
    if (props.hasOwnProperty('file')) {
      this.props.file = props.file
    }
    this.attributes = this.props.attributes.map((item) => {
      return item;
    });
    this.variables = this.props.variables.map((item) => {
      return item;
    });
    return etch.update(this)
  }

  renderAttributes () {
    return $.div(
      {},
      ...this.attributes.map((att, index) => $(ValueInputView, {
            ref: 'attributes',
            type: 'Attribute',
            name: att.name,
            value: att.value,
            text: att.text,
            className: 'texton-sections-settable padded',
            permitsFreeValue: false,
            items: att.items,
            showClearButton: true,
            onDidInitialize: this.didInitializeInput.bind(this),
            onDidConfirmSelection: this.didConfirmSelection.bind(this),
            onDidClickLink: this.didClickAttributeLink.bind(this)
        })));
  }

  renderVariables () {
    return $.div(
      {},
      ...this.variables.map((att, index) => $(ValueInputView, {
            ref: 'variables',
            type: 'Variable',
            name: att.name,
            value: att.value,
            text: att.text,
            className: 'texton-sections-settable padded',
            permitsFreeValue: att.permitsFreeValue,
            items: att.items,
            showClearButton: true,
            onDidInitialize: this.didInitializeInput.bind(this),
            onDidConfirmSelection: this.didConfirmSelection.bind(this),
            onDidClickLink: this.didClickVariableLink.bind(this)
        })));
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
          items : [],
          onDidInitialize: this.didInitializeChild.bind(this)
        }))
      )
    } else {
      return ""
    }
  }

  didInitializeChild (child) {
    this.children.push(child);
  }

  didClickVariableLink (variableName) {
    this.props.onDidClickSmartLink ('Variable', this.props.file, variableName)
    return false
  }

  didClickAttributeLink (attributeName) {
    this.props.onDidClickSmartLink ('Attribute', this.props.file, attributeName)
    return false
  }

  didInitializeSection (section) {
    this.sections.push(section);
  }

  didInitializeInput (input) {
    this.inputs.push(input);
  }

  didConfirmSelection (type, name, value) {
    this.props.onDidConfirmSelection (this.props.file, type, name, value)
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
    atom.commands.add(this.element,
      {
        'core:move-up': this.moveUp.bind(this),
        'core:move-down': this.moveDown.bind(this),
        'TextOn:browser-expand-collapsible': this.expandSelected.bind(this),
        'TextOn:browser-collapse-collapsible': this.collapseSelected.bind(this)
      });
  }

  scrollToEntry (entry) {
    if (entry != null) {
      entry.scrollIntoViewIfNeeded(true);
    }
  }

  expandSelected () {
    if (this.selectedEntry != null) {
      if (this.selectedEntry.browserNode != null) {
        this.selectedEntry.browserNode.expand();
      }
    }
  }

  collapseSelected () {
    if (this.selectedEntry != null) {
      if (this.selectedEntry.browserNode != null) {
        if (this.selectedEntry.browserNode.parentNode != null) {
          this.selectEntry(this.selectedEntry.browserNode.parentNode.element);
          this.selectedEntry.browserNode.collapse();
        }
      }
    }
  }

  selectEntry (entry) {
    Logger.logf('selectEntry', 'Entry', [entry])
    if (entry == null) {
      return null;
    }

    if (this.selectedEntry != null) {
      Logger.logf('selectEntry', 'Deselect', [this.selectedEntry])
      this.selectedEntry.classList.remove('selected');
    }

    Logger.logf('selectEntry', 'Select', [entry])
    this.selectedEntry = entry;
    entry.classList.add('selected');
    return entry;
  }

  entryClicked (e) {
    const entry = e.target.closest('.entry');
    this.selectEntry(entry);
    if (entry != null) {
      if (entry.classList.contains('texton-tree')) {
        entry.toggleExpansion();
      }
    }
  }

  moveDown (event) {
    event.stopImmediatePropagation();
    selectedEntry = this.selectedEntry;
    if (selectedEntry != null) {
      if (selectedEntry.classList.contains('texton-collapsible')) {
        if ((selectedEntry.children != null) &&
            (selectedEntry.children.length == 2) &&
            (this.selectEntry(selectedEntry.children[1].children[0]))) {
          this.scrollToEntry(this.selectedEntry);
          return;
        }
      }
      if (nextEntry = this.nextEntry(selectedEntry)) {
        this.selectEntry(nextEntry);
      }
    } else {
      this.selectEntry(this.roots[0]);
    }
    this.scrollToEntry(this.selectedEntry);
  }

  moveUp (event) {
    event.stopImmediatePropagation();
    selectedEntry = this.selectedEntry;
    if (selectedEntry != null) {
      if (previousEntry = this.previousEntry(selectedEntry)) {
        this.selectEntry(previousEntry);
//        if (previousEntry.classList.contains('texton-collapsible')) {
//          this.selectEntry(_.last(previousEntry.children[1].children));
//        }
      } else {
        this.selectEntry(selectedEntry.parentElement.closest('.texton-collapsible'));
      }
    } else {
      entries = this.list.querySelectorAll('.entry');
      this.selectEntry(entries[entries.length - 1]);
    }
    this.scrollToEntry(this.selectedEntry);
  }

  nextEntry (entry) {
    currentEntry = entry
    while (currentEntry != null) {
      if (currentEntry.nextSibling != null) {
        currentEntry = currentEntry.nextSibling
        if (currentEntry.matches('.entry')) {
          return currentEntry;
        }
      } else {
        currentEntry = currentEntry.parentElement.closest('.texton-collapsible');
      }
    }
    return null;
  }

  previousEntry (entry) {
    currentEntry = entry;
    while (currentEntry != null) {
      currentEntry = currentEntry.previousSibling;
      if ((currentEntry != null) && (currentEntry.matches('.entry'))) {
        return currentEntry;
      }
    }
    return null;
  }

  render () {
    return (
      <div className='texton-browser tool-panel' tabIndex='-1'>
        <main className='texton-sections'>
          <PaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='attributes' title='Attributes'>
            {this.renderAttributes()}
          </PaneSectionView>
          <PaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='variables' title='Variables'>
            {this.renderVariables()}
          </PaneSectionView>
          <PaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='browser' title='Browser'>
            {this.renderItems()}
          </PaneSectionView>
        </main>
      </div>
    )
  }
}
