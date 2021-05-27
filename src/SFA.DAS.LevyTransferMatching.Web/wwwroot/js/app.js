function SelectAllCheckboxes(selectAllCheckboxes) {
    this.checkBoxesWrap = selectAllCheckboxes;
    this.label = this.checkBoxesWrap.dataset.checkboxesSelectAllLabel || "Select all";
    this.createSelectAll()
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

    this.checkBoxesWrap.insertBefore(div, this.checkBoxesWrap.childNodes[0]);

  }

  SelectAllCheckboxes.prototype.toggleCheckboxes = function (e) {
      var selectAllChecked = e.target.checked;
      var checkboxes = this.checkBoxesWrap.querySelectorAll('input[type="checkbox"]');
      for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = selectAllChecked;
      }
  }






  var selectAllCheckboxes = document.querySelector('[data-checkboxes-select-all]')
  if (selectAllCheckboxes) {
    new SelectAllCheckboxes(selectAllCheckboxes);
  }