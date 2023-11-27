# WebEngine 2.0 RoadMap

## Element Description
### New Attributes for `WebElementDescription`
- `Type`: this attribute stands for DOM property `type`. It is usually used in `<input>` element.
- `Value`: this attribute stands for DOM Property Attribute `value`.
- `IsChecked`: this attribute stands for Dom Attribute `checked`, used for check box and radio button.
- `Href`: this attribute stands for Dom Attribute `href`, used for `<a>` element (Hyperlink)

- `IEnumerable<RelatedElements>`: a list of RelatedElements for related element identification.

### New Attributes for `AppElementDescription`
- To be defined.


## WebElements Objects
BaseWebElement is the base class for all web elements and it represents all kind of HTML elements.
In the Page Object Model system, all other elements are inherted from BaseWebElement.



### BaseWebElement
BaseWebElement provides following methods:
- Click() to perform click action on the element.
- Exists Property to check if the element exists.
- IsVisible Property to check if the element is visible in the page.
- IsInViewPort Property to check if the element is in the current view port.
- And other properties such as OuterHtml, InnerHtml and InnerText to feach the element's content.

- RunJavaScript(): runs a javascript code on the element.

BaseWebElement provides static `Describe` method or `Describe<T>` method to prodoce instances.

### TextBox
`TextBox` is inherted from `BaseWebElement.

#### Default element description:
- TagName = "input"
- Type = "text"


#### Additional methods:
- Clear() to clear the text in the textbox.
- SetValue(string) to set the text in the textbox (replaces current value)
- SendKeys(string) to send keys to the textbox (in addition to current value)

and extra properties such as:

### PasswordBox
`PasswordBox` is inherted from `TextBox`.

#### Default element description:
- Type = "password"

#### Additional methods:
- SetSecure() set the value to a password box by provinding an encrypted value.

### TextArea
`TextArea` is inherted from `BaseWebElement`. TextArea will have default element description:
- TagName = "textarea"
It works similar to 


### RadioGroup
`RadioGroup` represents a groupe of radio button shares the same Name attribute. RadioGroup represents multiple elements

#### Default element description:
- TagName = "input"
- Type = "radio"

It can be idenfieid with `Name`

#### Additional methods and properties:
- CheckByValue(string value) checks a radio button by its value.
- GetValue(): gets the value currently checked.
- IsChecked(string value): gets if the given value is checked.

### Select
`Select` represents a HTML ComboBox element witn multiple options. 

#### Default element description:
- TagName = "select"

#### Additional methods:
- SelectByText(string text) selects an option by its text.
- SelectByValue(string value) selects an option by its value.   


### CheckBox
`CheckBox` represents a HTML CheckBox element.

#### Default element description:
- TagName = "input"
- Type = "checkbox"

#### Additional methods/properties:
- Check() checks an checkbox
- Uncheck() unchecks an checkbox
- IsChecked checks if the checkbox is checked.


### Hyperlink
`Hyperlink` represents an `<a>` element.

#### Default element description:
- TagName = "a"

#### Additional methods/properties:
- LinkUrl gets the link url.


## Accessibility
### Uses Axe-core library
to be defined.