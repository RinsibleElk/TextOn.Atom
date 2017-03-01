/** @babel */
/** @jsx etch.dom */

import ComboboxView from './combobox-view'
import etch from 'etch'

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
    console.log('Jonas : we confirmed this:', item)
  }

  didCancelSelection () {
  }

  didChangeQuery (query) {
  }

  render () {
    return (
      <atom-panel className='popover'>
        <ComboboxView
          items={this.props.items}
          maxResults={this.props.maxResults}
          itemsClassList={['collapsed']}
          didChangeQuery={this.didChangeQuery.bind(this)}
          elementForItem={this.elementForItem.bind(this)}
          permitsFreeValue={this.props.permitsFreeValue}
          didConfirmSelection={this.didConfirmSelection.bind(this)}
          didCancelSelection={this.didCancelSelection.bind(this)} />
      </atom-panel>
    )
  }

  serialize () {
    return {
      deserializer: this.constructor.name,
    }
  }

  update () {

  }
}
