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

  linkClicked () {

  }

  // not sure about these class names yet...
  render () {
    return (
      <atom-panel className='padded'>
        <div class="inset-panel">
          <div class="panel-heading">
            <a>{this.props.name}</a>
          </div>
          <div class="panel-body padded">
            <div class='block'>
              <label>{this.props.text}</label>
            </div>
            <div class='block'>
              <ComboboxView
                value={this.props.value}
                items={this.props.items}
                didChangeQuery={this.didChangeQuery.bind(this)}
                elementForItem={this.elementForItem.bind(this)}
                permitsFreeValue={this.props.permitsFreeValue}
                didConfirmSelection={this.didConfirmSelection.bind(this)}
                didCancelSelection={this.didCancelSelection.bind(this)} />
            </div>
          </div>
        </div>
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
