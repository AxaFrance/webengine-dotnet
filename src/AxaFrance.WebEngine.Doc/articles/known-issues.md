# Known issues and possible workarounds

This page resumes some known issues which cannot be resolved at the level of Framework, or maybe you need to change system settings in order to run automated tests.

## GetAttribute, GetInnerHtml, GetOuterHtml, Value returns `null`
This issue is caused by a bug in chrome driver pre v92.0. To test your application, you may want to update the Chrome installed on your device so a recent version of chrome driver can be used. 

## IsVisibleInViewPort throws an Exception
This issue may occur in older version of mobile devices with older version of browser such as Chrome 69. To determine if an element is in the view port, the framework uses a JavaScript function `getBoundingClientRect()` which is not available in older version of browsers (such as chrome 69 installed with Android 9.0)

## Drag-and-Drop does not work on mobile browsers
This issue is known because mobile devices does not have mouse, the drag operation is often initialized by long-press.


## Edge shows Sign-in popup during the initialization of WebDriver
![Forced Signin Edge](../images/forced-signin-edge.png)

This issue occurs when your enterprise administrator forced users to sign-in while using the chromium-based Edge browser. You may ask the administrator to exclude your test machines from mandatory sign-in by modifying the GPO (Group Policy) or modifying Windows registry of your test machines:

* Key: `HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge`
* Value: `BrowserSignin`, Type: `DWORD`
* Data:
    * `Disabled` = 0
    * `Enabled` = (remove the value)
    * `Force` = 2

Remove the value or modify the data to 0 will disabled the forced sign-in
