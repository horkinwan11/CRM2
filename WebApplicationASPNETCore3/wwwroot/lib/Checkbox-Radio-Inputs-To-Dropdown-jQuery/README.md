# A jQuery plugin that converts a group of inputs type checkbox or radio into a bootstrap dropdown-menu
#### An advantage of this plugin is that you can have any dom structure of your groups with inputs and use it

[Demo page](https://lunev.github.io/input-to-dropdown/)

# Getting started
**Example of usage #1**

Type(checkbox), parent(table), children(td)
```html
<div class="checkbox-group">
    <table>
        <tr>
            <td><input id="value1" type="checkbox"><label for="value1">Value 1</label></td>
            <td><input id="value2" type="checkbox"><label for="value2">Value 2</label></td>
            <td><input id="value3" type="checkbox"><label for="value3">Value 3</label></td>
            <td><input id="value4" type="checkbox"><label for="value4">Value 4</label></td>
        </tr>
    </table>
</div>
<link rel="stylesheet" href="input-to-dropdown.css">
<script type="text/javascript" src="input-to-dropdown.js"></script>
<script type="text/javascript">inputsToDropdown('.checkbox-group', 'Select please', 'checkbox', '.checkbox-group', '.checkbox-group > table');</script>
<!--
'.checkbox-group'          - {string} A parent node of a group with inputs
'Select please'            - {string} A deafault name of your dropdown button
'checkbox'                 - {string} A type of a group with inputs
'.checkbox-group'          - {string} A node where you put your new dropdown
'.checkbox-group > table'  - {string} A group with inputs that you want to hide (Can be empty)
-->
```

**Example of usage #2**

Type(radio), parent(div.radio-group), children(div)
```html
<div class="radio-group">
    <div><label for="val9"><input name="radio1" type="radio"> Value 1</label></div>
    <div><label for="val10"><input name="radio1" type="radio"> Value 2</label></div>
    <div><label for="val11"><input name="radio1" checked="radio" type="radio"> Value 3</label></div>
    <div><label for="val12"><input name="radio1" type="radio"> Value 4</label></div>
</div>
<link rel="stylesheet" href="input-to-dropdown.css">
<script type="text/javascript" src="input-to-dropdown.js"></script>
<script type="text/javascript">inputsToDropdown('.radio-group', 'Select please', 'radio', '.radio-group', '');</script>
<!--
'.radio-group'          - {string} A parent node of a group with inputs
'Select please'         - {string} A deafault name of your dropdown button
'radio'                 - {string} A type of a group with inputs
'.radio-group'          - {string} A node where you put your new dropdown
''                      - {string} A group with inputs that you want to hide (Can be empty)
-->
```

## Authors

* **Alex Lunev** - *Initial work* - [Alex Lunev](https://github.com/lunev)


