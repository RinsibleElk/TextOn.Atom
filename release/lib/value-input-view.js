/** @babel */
/** @jsx etch.dom */

import ComboboxView from './combobox-view'
import etch from 'etch'

export default class ValueInputView {
  constructor (props) {
    this.props = props
    this.combobox = null;
    etch.initialize(this)
    if (props.onDidInitialize) {
      props.onDidInitialize(this)
    }
  }

  elementForItem (item) {
    const li = document.createElement('li');
    const span = document.createElement('span');
    span.textContent = item;
    li.appendChild(span);
    return li
  }

  didConfirmSelection (item) {
    if (this.props.onDidConfirmSelection) {
      this.props.onDidConfirmSelection(this.props.type, this.props.name, item)
    }
  }

  didClickClear () {
    if (this.props.onDidConfirmSelection) {
      this.props.onDidConfirmSelection(this.props.type, this.props.name, '')
    }
  }

  didCancelSelection () {
  }

  didChangeQuery (query) {
  }

  linkClicked () {

  }

  didInitializeCombobox (combobox) {
    if (this.combobox != null) {
      throw 'No combobox created'
    }
    this.combobox = combobox;
  }

  destroy () {
    if (this.combobox != null) {
      this.combobox.destroy();
    }
    this.combobox = null;
    return etch.destroy(this);
  }

  didClickLink () {
    this.props.onDidClickLink(this.props.name)
    return false;
  }

  renderClearButton() {
    if (this.props.showClearButton) {
      return (
        <button class='btn btn' onClick={this.didClickClear.bind(this)}>Clear</button>
      )
    }
    else {
      return ""
    }
  }

  render () {
    return (
      <atom-panel className={this.props.className}>
        <div class="inset-panel">
          <div class="panel-heading">
            <a onClick={this.didClickLink.bind(this)}>{this.props.name}</a>
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
            {this.renderClearButton()}
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

  update (props) {
    this.props = props
    this.combobox.update ({
      value : this.props.value,
      items : this.props.items
    })
    etch.update(this)
  }
}
