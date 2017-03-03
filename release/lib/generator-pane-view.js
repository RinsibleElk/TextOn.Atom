/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import ValueInputSectionView from './value-input-section-view'
import ValueInputView from './value-input-view'
import TextOnCore from './texton-core'

export default class GeneratorPaneView {
  constructor (props) {
    this.collapsedSections = props.collapsedSections ? new Set(props.collapsedSections) : new Set();
    this.inputs = [];
    this.sections = [];
    this.props = props;
    etch.initialize(this);
    for (const section of this.sections) {
      if (this.collapsedSections.has(section.name)) {
        section.collapse();
      } else {
        section.expand();
      }
    }
  }

  destroy () {
    for (const input of this.inputs) {
      input.destroy();
    }
    for (const section of this.sections) {
      section.destroy();
    }
    this.inputs = null;
    this.sections = null;
  }

  serialize () {
    return {
      deserializer: this.constructor.name,
      collapsedSections: this.sections.filter((s) => s.collapsed).map((s) => s.name)
    }
  }

  update (props) {
    if (props.hasOwnProperty('attributes')) {
      this.props.attributes = props.attributes
    }
    if (props.hasOwnProperty('variables')) {
      this.props.variables = props.variables
    }
    return etch.update(this)
  }

  getTitle () {
    return 'TextOn Generator';
  }

  didInitializeSection (section) {
    this.sections.push(section);
  }

  didInitializeInput (input) {
    this.inputs.push(input);
  }

  didConfirmSelection (type, name, value) {
    console.log('Selected: ', type, name, value)
  }

  isEqual (other) {
    return other instanceof GeneratorPaneView;
  }

  renderAttributes () {
    return $.div(
      {},
      ...this.props.attributes.map((att, index) => $(ValueInputView, {
            type: 'Attribute',
            name: att.name,
            value: att.value,
            text: att.text,
            className: 'texton-sections-settable padded',
            permitsFreeValue: false,
            items: att.items,
            onDidInitialize: this.didInitializeInput.bind(this),
            onDidConfirmSelection: this.didConfirmSelection.bind(this)
        })));
  }

  renderVariables () {
    return $.div(
      {},
      ...this.props.variables.map((att, index) => $(ValueInputView, {
            type: 'Variable',
            name: att.name,
            value: att.value,
            text: att.text,
            className: 'texton-sections-settable padded',
            permitsFreeValue: att.permitsFreeValue,
            items: att.items,
            onDidInitialize: this.didInitializeInput.bind(this),
            onDidConfirmSelection: this.didConfirmSelection.bind(this)
        })));
  }

  render () {
    return (
      <div className='texton-generator pane-item' tabIndex='-1'>
        <header className='texton-header'>
          <h1>TextOn Generator</h1>
        </header>
        <main className='texton-sections'>
          <ValueInputSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='attributes' title='Attributes'>
            {this.renderAttributes()}
          </ValueInputSectionView>
          <ValueInputSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='variables' title='Variables'>
            {this.renderVariables()}
          </ValueInputSectionView>
        </main>
      </div>
    )
  }
}
