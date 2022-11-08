# Development Status of the Framework

## Components of WebEngine Framework

|Component|.NET|Java|Description|
|---------|----|----|-----------|
|AXA.WebEngine|🟢|🟢|Provide basic data structure used in an automation project|
|AXA.WebEngine.Web|🟢|🟢|Provide functionalities to test Web based application on Desktop and Mobile browsers (Based on Selenium 4)|
|AXA.WebEngine.MobileApp|🟢|🔘|Provide functionalities to test Native and Hybrid mobile applications on Android and iOS (Based on Appium)|
|AXA.WebEngine.WebRunner|🟢|🟢|Run automated tests using Keyword driven testing approach|
|AXA.WebEngine.ReportViewer|🟢|🟢|View test reports (unique version for both .NET and Java version of the framework)|

## Actions for Web Elements (Via <xref:AxaFrance.WebEngine.Web.WebElementDescription>)
These actions are available for WebElementDescription. Please note that some actions have different meaning on Desktop and Mobile browsers.

`Synchronized` signifies that the action is secured by the Framework. If the action failed because of page reloading or DOM updating by JavaScript programs, actions will be automatically retried. This feature significantly improves the readability of the test script (code) and reduces maintenance effort.



|Action|.NET|Java|Description|Synchronized|
|---------|----|----|-----------|--------|
|FindElement()|✔||Finds a unique Web Element with current element description.|✔|
|FindElement(By)|✔| |Finds an sub-element of the current Web Element|✔|
|FindElements()|✔||Finds one or more Web Elements with current element description.(for example options of a `select` element or buttons in the same `radio button group`)|✔|
|FindElements(By)|✔||Finds one or more sub-element of the current unique Web Element|✔|
|Exists()|✔||Checks if an Web Element exists on the DOM|✔|
|Clear()|✔||Clears the value of the element (for text-boxes, text-areas and password-boxes)|✔|
|Click()|✔||Clicks on the Web Element|✔|
|CheckByValue(String)|✔||Checks an option of radio button group based on html attribute `value`. The current description corresponds to all radio buttons of the same group|✔|
|DragAndDropTo(ElementDescription)|✔||Drags the current element and drops to another element. (works only on desktop browser)|✔|
|GetAttribute(string)|✔||Gets the value of the given html attribute|✔|
|GetInnerHtml()|✔||Gets the value of the html attribute `innerHTML`|✔|
|GetOuterHtml()|✔||Gets the value of the html attribute `outerHTML`|✔|
|GetScreenshot()|✔||Generates a screenshot of the current web page||
|MouseHover()|✔||Hovers the mouse on a given element (Desktop only. on mobile devices, a Click will be performed instead.)|✔|
|RightClick()|✔||Preforms right-click on an element. On Mobile devices, an long touch will be performed|✔|
|ScrollIntoView()|✔||Scrolls the screen until the element is shown in the current view port||
|SendKeys(string)|✔||Sends the text to textbox based elements (for text-box, text-area and password-box)|✔|
|SetSecure(String)|✔||Takes a crypted data, and set the value to a password-box (works only on password-box)|✔|
|SetValue(string)|✔||Clear the current value of textbox based elements and replace with provided value (for text-box, text-area and password-box)|✔|
|GetText()|✔||Gets the text property of the element (for text-box and text-area)|✔|
|Value|✔||Gets the `value` attribute of the element (usually in html forms)|✔|
|IsSelected|✔||Checks if the Web Element is selected. Applies only on checkboxes, options in a select element and radio buttons|✔|
|IsEnabled|✔||Checks if the Web Element is enabled||
|IsDisplayed|✔||Checks if the Web Element is visible (Either in viewport or not on mobile devices)||
|IsVisibleInViewPort|✔||Checks if the Web Element is visible in the current view port||
|AsSelect()|✔||Converts the Web Element into a `SelectElement`||
|SelectByIndex(Int32)|✔||Considers current Web Element is `select` and choose an option based on index|✔|
|SelectByText(String)|✔||Considers current Web Element is `select` and choose an option based on displayed text|✔|
|SelectByValue(String)|✔||Considers current Web Element is `select` and choose an option based on `value` attribute|✔|


