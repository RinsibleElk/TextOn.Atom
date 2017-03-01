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
          <ValueInputView maxResults={2} items={['one', 'two', 'three', 'four', 'five', 'six']} />
          <ValueInputView maxResults={1} items={['eight']} />
        </main>
      </div>
    )
  }
}
