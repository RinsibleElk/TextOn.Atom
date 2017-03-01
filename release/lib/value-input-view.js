/** @babel */
/** @jsx etch.dom */

import SelectListView from 'atom-select-list'
import etch from 'etch'
import dedent from 'dedent'

export default class ValueInputView {
  constructor () {
    etch.initialize(this)
  }

  elementForItem (item) {
    const li = document.createElement('li')
    li.textContent = item
    return li
  }

  didConfirmSelection (item) {
    console.log('confirmed', item)
  }

  didCancelSelection () {
    console.log('cancelled')
  }

  render () {
    return (
      <atom-panel className='popover'>
        <SelectListView
          items={['one', 'two', 'three', 'four', 'five', 'six']}
          maxResults={2}
          elementForItem={this.elementForItem.bind(this)}
          onDidConfirmSelection={this.didConfirmSelection.bind(this)}
          onDidCancelSelection={this.didCancelSelection.bind(this)} />
      </atom-panel>
    )
  }

  update () {

  }
}
