﻿// Select-all Checkboxes

function SelectAllCheckboxes(selectAllCheckboxes) {
  this.checkBoxesWrap = selectAllCheckboxes;
  this.checkBoxes = this.checkBoxesWrap.querySelectorAll('input[type="checkbox"]');
  this.label = this.checkBoxesWrap.dataset.checkboxesSelectAllLabel || "Select all";
  for (var i = 0; i < this.checkBoxes.length; i++) {
    this.checkBoxes[i].addEventListener('click', this.updateSelectAllCheckbox.bind(this))
  }
  this.createSelectAll();
  this.updateSelectAllCheckbox();
}

SelectAllCheckboxes.prototype.updateSelectAllCheckbox = function () {
  var checkbox = this.selectAllCheckBox.querySelector('input');
  checkbox.checked = this.areAllTheCheckboxesChecked();
}

SelectAllCheckboxes.prototype.areAllTheCheckboxesChecked = function () {
  var noOfChecked = 0;
  for (var i = 0; i < this.checkBoxes.length; i++) {
    if (this.checkBoxes[i].checked) {
      noOfChecked++
    }
  }
  return noOfChecked === this.checkBoxes.length
}

SelectAllCheckboxes.prototype.createSelectAll = function () {
  var selectAllID = "app-checkbox-select-all"
  var div = document.createElement('div');
      div.className = "govuk-checkboxes__item app-checkboxes__select-all"
  var checkbox = document.createElement('input');
      checkbox.type = "checkbox";
      checkbox.className = "govuk-checkboxes__input";
      checkbox.id = selectAllID;
      checkbox.addEventListener('click', this.toggleCheckboxes.bind(this))
  var label = document.createElement('label');
      label.innerText = this.label;
      label.htmlFor = selectAllID;
      label.className = "govuk-label govuk-checkboxes__label";

  div.appendChild(checkbox);
  div.appendChild(label);

  this.selectAllCheckBox = div;
  this.checkBoxesWrap.insertBefore(this.selectAllCheckBox, this.checkBoxesWrap.childNodes[0]);
}

SelectAllCheckboxes.prototype.toggleCheckboxes = function (e) {
    var selectAllChecked = e.target.checked;
    var checkboxes = this.checkBoxesWrap.querySelectorAll('input[type="checkbox"]');
    for (var i = 0; i < checkboxes.length; i++) {
      checkboxes[i].checked = selectAllChecked;
    }
}


// Location Autocomplete

var locationInputs = document.querySelectorAll(".app-location-autocomplete");
var apiUrl = '/locations';

if (locationInputs.length > 0) {

  for (var i = 0; i < locationInputs.length; i++) {

    var input = locationInputs[i]
    var container = document.createElement('div');

    container.className = "das-autocomplete-wrap"
    input.parentNode.replaceChild(container, input);

    var getSuggestions = function (query, updateResults) {
      var results = [];
      var xhr = new XMLHttpRequest();
      xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) {
          var jsonResponse = JSON.parse(xhr.responseText);
          results = jsonResponse.locations.map(function (r) {
            return r.name;
          });
          updateResults(results);
        }
      }
      xhr.open("GET", apiUrl + '?searchTerm=' + query, true);
      xhr.send();
    };

    accessibleAutocomplete({
      element: container,
      id: input.id,
      name: input.name,
      defaultValue: input.value,
      displayMenu: 'overlay',
      showNoOptionsFound: false,
      minLength: 2,
      source: getSuggestions,
      placeholder: "",
      confirmOnBlur: false,
      autoselect: true
    });
  }
}


// Show/Hide Extra Location Fields

function ExtraFieldRows(container) {

  this.container = container
  this.firstField = this.container.querySelector('input[type=text]')
  this.fieldset = this.container.querySelector('.app-extra-fields__fieldset')
  this.extraFieldRows = this.fieldset.querySelectorAll('.app-extra-fields__form-group')
  this.allLocations = this.container.querySelector('.app-extra-fields__all-locations')
  this.hiddenClass = 'app-extra-field__form-group--hidden'

  this.addLink()
  this.allLocationsCheckboxEvent()

  this.firstField.addEventListener('change', this.clearAllLocationsCheckbox.bind(this))

  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    var textInput = extraFieldRow.querySelector('input[type=text]')
    this.addRemoveLink(extraFieldRow)
    if (textInput.value === '') {
      this.hideRow(extraFieldRow)
    }
  }
}

ExtraFieldRows.prototype.addLink = function () {
  var addRowLink = document.createElement('a');
  addRowLink.innerHTML = 'Add another location'
  addRowLink.className = 'govuk-link govuk-link--no-visited-state app-extra-field__form-group-link--add'
  addRowLink.href = '#'
  addRowLink.addEventListener('click', this.showFirstAvailableRow.bind(this))
  this.fieldset.parentNode.insertBefore(addRowLink, this.fieldset.nextSibling);
}

ExtraFieldRows.prototype.showFirstAvailableRow = function () {
  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    if (extraFieldRow.classList.contains(this.hiddenClass)) {
      this.showRow(extraFieldRow)
      break;
    }
  }
}

ExtraFieldRows.prototype.allLocationsCheckboxEvent = function () {
  var that = this
  if (this.allLocations) {
    this.allLocations.addEventListener('click', function () {
      if (this.checked) {
        var firstField = that.container.querySelector('input[type=text]')
        firstField.value = ""
        for (var f = 0; f < that.extraFieldRows.length; f++) {
          var extraFieldRow = that.extraFieldRows[f]
          that.hideRow(extraFieldRow)
        }
      }
    })
  }
}

ExtraFieldRows.prototype.clearAllLocationsCheckbox = function () {
  this.allLocations.checked = false
}

ExtraFieldRows.prototype.addRemoveLink = function (row) {
  var that = this
  var removeLink = document.createElement('a');
  removeLink.innerHTML = 'Remove'
  removeLink.className = 'govuk-link govuk-link--no-visited-state app-extra-field__form-group-link--remove'
  removeLink.href = "#"
  removeLink.addEventListener('click', function(){ that.hideRow(row); })
  row.append(removeLink)
}

ExtraFieldRows.prototype.hideRow = function (row) {
  var textInput = row.querySelector('input[type=text]')
  textInput.value = ''
  row.classList.add(this.hiddenClass)
}

ExtraFieldRows.prototype.showRow = function (row) {
  this.clearAllLocationsCheckbox()
  row.classList.remove(this.hiddenClass)
}



// App 

var selectAllCheckboxes = document.querySelector('[data-checkboxes-select-all]')
if (selectAllCheckboxes) {
  new SelectAllCheckboxes(selectAllCheckboxes);
}

var extraFieldRows = document.querySelector('[data-extra-field-rows]')
if (extraFieldRows) {
  new ExtraFieldRows(extraFieldRows);
}