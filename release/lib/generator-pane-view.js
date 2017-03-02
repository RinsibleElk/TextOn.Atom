/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
import ValueInputView from './value-input-view'

export default class GeneratorPaneView {
  constructor (props) {
    this.collapsedSections = props.collapsedSections ? new Set(props.collapsedSections) : new Set();
    this.sections = [];
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
    // intentionally empty.
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

  render () {
    return (
      <div className='texton-generator pane-item' tabIndex='-1'>
        <header className='texton-header'>
          <h1>TextOn Generator</h1>
        </header>
        <main className='texton-sections'>
          <div class='block'>
            <p>This one does not permit free value.</p>
            <ValueInputView
              className='texton-sections-settable'
              permitsFreeValue={false}
              items={['one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight','nine','ten','eleven']} />
          </div>
          <div class='block'>
            <p>This one does permit free value.</p>
            <ValueInputView
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={['eight','nine','ten','eleven']} />
          </div>
          <div class='block'>
          <p>This one permits free value and has no suggestions.</p>
            <ValueInputView
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={[]} />
          </div>
          <div class='block'>
            <p>This one does not permit free value. Start value: 'three'.</p>
            <ValueInputView
              value='three'
              className='texton-sections-settable'
              permitsFreeValue={false}
              items={['one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight','nine','ten','eleven']} />
          </div>
          <div class='block'>
            <p>This one does permit free value. Start value: 'eighteen'.</p>
            <ValueInputView
              value='eighteen'
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={['eight','nine','ten','eleven']} />
          </div>
          <div class='block'>
            <p>This one permits free value and has no suggestions. Start value: 'Elk'.</p>
            <ValueInputView
              value='Elk'
              className='texton-sections-settable'
              permitsFreeValue={true}
              items={[]} />
          </div>
        </main>
      </div>
    )
  }
}
