/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
const $ = etch.dom
import GeneratorPaneSectionView from './generator-pane-section-view'
import ValueInputView from './value-input-view'
import TextOnCore from './texton-core'

export default class GeneratorPaneView {
  constructor (props) {
    this.collapsedSections = props.collapsedSections ? new Set(props.collapsedSections) : new Set();
    this.inputs = [];
    this.sections = [];
    this.attributes = [];
    this.variables = [];
    this.output = [];
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
    if (props.hasOwnProperty('functionName')) {
      this.props.functionName = props.functionName
    }
    if (props.hasOwnProperty('fileName')) {
      this.props.fileName = props.fileName
    }
    if (props.hasOwnProperty('canGenerate')) {
      this.props.canGenerate = props.canGenerate
    }
    if (props.hasOwnProperty('output')) {
      this.props.output = props.output
    }
    this.attributes = this.props.attributes.map((item) => {
      return item;
    });
    this.variables = this.props.variables.map((item) => {
      return item;
    });
    this.output = this.props.output.map((item) => {
      return item;
    });
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
    this.props.onDidConfirmSelection (type, name, value)
  }

  isEqual (other) {
    return other instanceof GeneratorPaneView;
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
            onDidInitialize: this.didInitializeInput.bind(this),
            onDidConfirmSelection: this.didConfirmSelection.bind(this),
            onDidClickLink: this.didClickVariableLink.bind(this)
        })));
  }

  renderOutput () {
    return $.div(
      {}
    )
  }

  didClickCopyToClipboard() {

  }

  didClickGenerate() {
    console.log('Clicked generate.')
    this.props.onDidClickGenerate();
  }

  renderGenerator () {
    if (this.props.canGenerate) {
      if (this.props.output.length === 0) {
        return (
          <div class='block'>
            <button class='btn btn-lg' onClick={this.didClickGenerate.bind(this)}>Generate</button>
          </div>
        );
      } else {
        return (
          <div class='block'>
            <button class='btn btn-lg' onClick={this.didClickGenerate.bind(this)}>Generate</button>
            {this.renderOutput()}
            <button class='btn' onClick={this.didClickCopyToClipboard.bind(this)}>Copy to clipboard</button>
          </div>
        );
      }
    } else {
      return (
        <p>Nothing to generate.</p>
      )
    }
  }

  didClickFunctionLink () {
    this.props.onDidClickSmartLink ('Function', this.props.fileName, this.props.functionName)
    return false
  }

  didClickVariableLink (variableName) {
    this.props.onDidClickSmartLink ('Variable', this.props.fileName, variableName)
    return false
  }

  didClickAttributeLink (attributeName) {
    this.props.onDidClickSmartLink ('Attribute', this.props.fileName, attributeName)
    return false
  }

  render () {
    return (
      <div className='texton-generator pane-item' tabIndex='-1'>
        <header className='texton-header'>
          <h1>TextOn Generator for <a onClick={this.didClickFunctionLink.bind(this)}>{this.props.functionName}</a></h1>
        </header>
        <main className='texton-sections'>
          <GeneratorPaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='attributes' title='Attributes'>
            {this.renderAttributes()}
          </GeneratorPaneSectionView>
          <GeneratorPaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='variables' title='Variables'>
            {this.renderVariables()}
          </GeneratorPaneSectionView>
          <GeneratorPaneSectionView onDidInitialize={this.didInitializeSection.bind(this)} name='generator' title='Generator'>
            {this.renderGenerator()}
          </GeneratorPaneSectionView>
        </main>
      </div>
    )
  }
}
