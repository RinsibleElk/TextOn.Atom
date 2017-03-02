/** @babel */
/** @jsx etch.dom */

import ComboboxView from './combobox-view'
import etch from 'etch'

export default class ValueInputView {
  constructor (props) {
    this.props = props
    this.comboboxes = []
    etch.initialize(this)
    if (props.onDidInitialize) {
      props.onDidInitialize(this)
    }
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

  didInitializeCombobox (combobox) {
    this.comboboxes.push(combobox);
  }

  destroy () {
    for (const combobox of this.comboboxes) {
      combobox.destroy();
    }
    this.comboboxes = null;
    return etch.destroy(this);
  }

  // not sure about these class names yet...
  render () {
    return (
      <atom-panel className={this.props.className}>
        <div class="inset-panel">
          <div class="panel-heading">
            <a>{this.props.name}</a>
          </div>
          <div class="panel-body">
            <label>{this.props.text}</label>
            <ComboboxView
              value={this.props.value}
              items={this.props.items}
              onDidInitialize={this.didInitializeCombobox.bind(this)}
              didChangeQuery={this.didChangeQuery.bind(this)}
              elementForItem={this.elementForItem.bind(this)}
              permitsFreeValue={this.props.permitsFreeValue}
              didConfirmSelection={this.didConfirmSelection.bind(this)}
              didCancelSelection={this.didCancelSelection.bind(this)} />
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
