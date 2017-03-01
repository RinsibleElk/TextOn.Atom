/** @babel */
/** @jsx etch.dom */

import etch from 'etch'
import ValueInputView from './value-input-view'

export default class GeneratorPaneView {
  constructor () {
    etch.initialize(this)
  }

  destroy () {
  }

  serialize () {
    return {
      deserializer: this.constructor.name,
    }
  }

  update () {
    // intentionally empty.
  }

  getTitle () {
    return 'TextOn Generator'
  }

  render () {
    return (
      <div className='texton texton-generator pane-item' tabIndex='-1'>
        <header className='texton-header'>
          <h1>TextOn Generator</h1>
        </header>
        <main className='texton-sections'>
          <p>This one does not permit free value.</p>
          <ValueInputView
            className='texton-sections-settable'
            maxResults={4}
            permitsFreeValue={false}
            items={['one', 'two', 'three', 'four', 'five', 'six']} />
          <p>This one does permit free value.</p>
          <ValueInputView
            className='texton-sections-settable'
            permitsFreeValue={true}
            maxResults={4}
            items={['eight','nine','ten','eleven']} />
        </main>
      </div>
    )
  }
}
