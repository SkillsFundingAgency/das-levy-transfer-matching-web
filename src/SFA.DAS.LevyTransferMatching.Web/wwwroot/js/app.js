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

const selectAllCheckboxes = document.querySelector('[data-checkboxes-select-all]')
if (selectAllCheckboxes) {
  new SelectAllCheckboxes(selectAllCheckboxes);
}

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
                var titleField = $('#SelectedStandardTitle');
                if (that.val().length === 0) {
                    var fieldId = that.attr('id'),
                        selectField = $('#' + fieldId + '-select');
                    selectField[0].selectedIndex = 0;
                    titleField[0].value = '';
                }
                titleField[0].value = that.val();
            });

        });
}

forms.attr('novalidate', 'novalidate');