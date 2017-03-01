/** @babel */
/** @jsx etch.dom */

import SelectListView from 'atom-select-list'
import etch from 'etch'
import dedent from 'dedent'

export default class ValueInputView {
  constructor (props) {
    this.props = props
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

  didChangeQuery (query) {
    console.log("current query is " + query)
  }

  render () {
    return (
      <atom-panel className='popover'>
        <SelectListView
          items={['one', 'two', 'three', 'four', 'five', 'six']}
          maxResults={this.props.maxResults}
          didChangeQuery={this.didChangeQuery.bind(this)}
          elementForItem={this.elementForItem.bind(this)}
          onDidConfirmSelection={this.didConfirmSelection.bind(this)}
          onDidCancelSelection={this.didCancelSelection.bind(this)} />
      </atom-panel>
    )
  }

  update () {

  }
}
