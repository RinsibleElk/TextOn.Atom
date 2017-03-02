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
    for (const section of this.sections) {
      section.destroy();
    }
    this.sections = null;
  }

  serialize () {
    return {
      deserializer: this.constructor.name,
      collapsedSections: this.sections.filter((s) => s.collapsed).map((s) => s.name)
    }
  }

  update () {

    return etch.update(this)
  }

  getTitle () {
    return 'TextOn Generator';
  }

  didInitializeSection (section) {
    this.sections.push(section);
  }

  isEqual (other) {
    return other instanceof GeneratorPaneView;
  }

  renderAttributes () {
    return $.div(
      {},
      ...this.props.attributes.map((att, index) => $(ValueInputView, {
            name: att.name,
            value: att.value,
            text: att.text,
            className: 'texton-sections-settable',
            permitsFreeValue: false,
            items: att.items
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
            <ValueInputView
              name='Number 4'
              text='This one does not permit free value. Start value: three.'
              value='three'
              className='texton-sections-settable'
              permitsFreeValue={false}
              items={['one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight','nine','ten','eleven']} />
            <ValueInputView
              name='Number 5'
              text='This one does permit free value. Start value: eighteen.'
              value='eighteen'
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={['eight','nine','ten','eleven']} />
            <ValueInputView
              name='Number 6'
              text='This one permits free value and has no suggestions. Start value: Elk.'
              value='Elk'
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={[]} />
          </ValueInputSectionView>
        </main>
      </div>
    )
  }
}
