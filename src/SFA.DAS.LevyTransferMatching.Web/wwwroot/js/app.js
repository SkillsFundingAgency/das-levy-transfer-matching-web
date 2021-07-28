// Select-all Checkboxes

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
var apiUrl = '/location';

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

  var autocompleteInputs = document.querySelectorAll(".autocomplete__input");
  if (autocompleteInputs.length > 0) {
    for (var i = 0; i < autocompleteInputs.length; i++) {
      var input = autocompleteInputs[i];
      input.setAttribute("autocomplete", "new-password");
    }
  }
}


// Show/Hide Extra Form Fields

function ExtraFieldRows(container) {
  this.container = container
  this.firstField = this.container.querySelector('input[type=text]')
  this.fieldset = this.container.querySelector('.app-extra-fields__fieldset')
  this.extraFieldRows = this.fieldset.querySelectorAll('.app-extra-fields__form-group')
  this.allCheckbox = this.container.querySelector('.app-extra-fields__all-checkbox')
  this.hiddenClass = 'app-extra-field__form-group--hidden'
  this.addButtonText = this.container.dataset.addButtonText || 'Add another'
}

ExtraFieldRows.prototype.init = function () {
  this.insertAddLink()
  if (this.allCheckbox) {
    this.allCheckboxEvent()
  }
  this.firstField.addEventListener('change', this.clearAllCheckbox.bind(this))

  // Append the remove links 
  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    this.appendRemoveLink(extraFieldRow)
  }

  // If all rows are hidden, add class to hide the fieldset
  var hiddenRowCount = this.showHideEmptyRows()
  if (hiddenRowCount === this.extraFieldRows.length) {
    this.fieldset.classList.add('app-extra-fields__fieldset--all-hidden')
  }
}

ExtraFieldRows.prototype.showHideEmptyRows = function () {
  var hiddenRowCount = 0
  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    var textInput = extraFieldRow.querySelector('input') 
    if (textInput.value === '') {
      this.hideRow(extraFieldRow)
      hiddenRowCount++;
    } else {
      this.showRow(extraFieldRow, false)
    }
  }
  return hiddenRowCount;
}

ExtraFieldRows.prototype.insertAddLink = function () {
  var addRowLink = document.createElement('a');
  addRowLink.innerHTML = this.addButtonText
  addRowLink.className = 'govuk-link govuk-link--no-visited-state app-extra-field__form-group-link--add'
  addRowLink.href = '#'
  addRowLink.addEventListener('click', this.showFirstAvailableRow.bind(this))
  this.addLink = addRowLink
  this.fieldset.parentNode.insertBefore(this.addLink, this.fieldset.nextSibling);
}

ExtraFieldRows.prototype.appendRemoveLink = function (row) {
  var that = this
  var removeLink = document.createElement('a');
  removeLink.innerHTML = 'Remove'
  removeLink.className = 'govuk-link govuk-link--no-visited-state app-extra-field__form-group-link--remove'
  removeLink.href = "#"
  removeLink.addEventListener('click',
    function(e) {
      e.preventDefault();
      that.addLink.classList.remove(that.hiddenClass)
      that.hideRow(row);
      that.updateRowOrder();
  })
  row.append(removeLink)
}

ExtraFieldRows.prototype.showFirstAvailableRow = function (e) {
  var hiddenRowCount = 0
  var rowToShow
  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    if (extraFieldRow.classList.contains(this.hiddenClass)) {
      if (hiddenRowCount === 0) {
        rowToShow = extraFieldRow
      }
      hiddenRowCount++
    }
  }
  if (hiddenRowCount === 1) {
    this.addLink.classList.add(this.hiddenClass)
  }
  this.showRow(rowToShow, true)
  e.preventDefault()
}

ExtraFieldRows.prototype.updateRowOrder = function () {
  var that = this
  var answers = [];
  for (var f = 0; f < that.extraFieldRows.length; f++) {
    var extraFieldRow = that.extraFieldRows[f]
    var textValue = extraFieldRow.querySelector('input').value
    if (!extraFieldRow.classList.contains(this.hiddenClass)) {
      answers.push(textValue)
    }
    extraFieldRow.querySelector('input').value = ''
    that.hideRow(extraFieldRow)
  }
  answers.forEach(function(answer, i) {
    var extraFieldRow = that.extraFieldRows[i]
    var textInput = extraFieldRow.querySelector('input')
    textInput.value = answer
    that.showRow(extraFieldRow, false)
  });
}

ExtraFieldRows.prototype.allCheckboxEvent = function () {
  var that = this
  if (this.allCheckbox) {
    this.allCheckbox.addEventListener('click', function () {
      if (this.checked) {
        var firstField = that.container.querySelector('input')
        firstField.value = ""
        that.addLink.classList.remove(that.hiddenClass)
        for (var f = 0; f < that.extraFieldRows.length; f++) {
          var extraFieldRow = that.extraFieldRows[f]
          that.hideRow(extraFieldRow)
        }
      }
    })
  }
}

ExtraFieldRows.prototype.clearAllCheckbox = function () {
  if (this.allCheckbox) {
    this.allCheckbox.checked = false
  }
}

ExtraFieldRows.prototype.hideRow = function (row) {
  var textInput = row.querySelector('input')
  textInput.value = ''
  row.classList.add(this.hiddenClass)
  if (this.areAllRowsHidden()) {
    this.fieldset.classList.add('app-extra-fields__fieldset--all-hidden')
  } else {
    this.fieldset.classList.remove('app-extra-fields__fieldset--all-hidden')
  }
}

ExtraFieldRows.prototype.showRow = function (row, focus = false) {
  var errorMessage = row.querySelector('.govuk-error-message')
  var textInput = row.querySelector('input')
  this.clearAllCheckbox()
  this.fieldset.classList.remove('app-extra-fields__fieldset--all-hidden')
  if (errorMessage) {
    errorMessage.remove()
  }
  row.classList.remove('govuk-form-group--error')
  row.classList.remove(this.hiddenClass)
  if (focus) {
    textInput.focus()
  }
}

ExtraFieldRows.prototype.areAllRowsHidden = function () {
  var hiddenRowCount = 0
  for (var f = 0; f < this.extraFieldRows.length; f++) {
    var extraFieldRow = this.extraFieldRows[f]
    if (extraFieldRow.classList.contains(this.hiddenClass)) {
      hiddenRowCount++
    }
  }
  return hiddenRowCount === this.extraFieldRows.length
}



// App 

function nodeListForEach(nodes, callback) {
  if (window.NodeList.prototype.forEach) {
    return nodes.forEach(callback)
  }
  for (var i = 0; i < nodes.length; i++) {
    callback.call(window, nodes[i], i, nodes);
  }
}

var selectAllCheckboxes = document.querySelector('[data-checkboxes-select-all]')
if (selectAllCheckboxes) {
  var selectAllFormControl = new SelectAllCheckboxes(selectAllCheckboxes);
}

var extraFieldRows = document.querySelectorAll('[data-extra-field-rows]');
nodeListForEach(extraFieldRows, function (extraFieldRows) {
  new ExtraFieldRows(extraFieldRows).init();
});



var forms = $('.validate-auto-complete');
var idSelectField = 'SelectedStandardId';
var selectEl = document.querySelector('#' + idSelectField);

if (selectEl) {
    accessibleAutocomplete.enhanceSelectElement({
        selectElement: selectEl,
        minLength: 3,
        autoselect: true,
        defaultValue: '',
        displayMenu: 'overlay',
        placeholder: '',
        onConfirm: function (opt) {
            var txtInput = document.querySelector('#' + idSelectField);
            var searchString = opt || txtInput.value;
            var requestedOption = [].filter.call(this.selectElement.options,
                function (option) {
                    return (option.textContent || option.innerText) === searchString
                }
            )[0];
            if (requestedOption) {
                requestedOption.selected = true;
            } else {
                this.selectElement.selectedIndex = 0;
            }
        }

    });
    forms.on('submit',
        function (e) {

            $('.autocomplete__input').each(function () {
                var that = $(this);
                if (that.val().length === 0) {
                    var fieldId = that.attr('id'),
                        selectField = $('#' + fieldId + '-select');
                    selectField[0].selectedIndex = 0;
                }
            });

        });
}

forms.attr('novalidate', 'novalidate');
