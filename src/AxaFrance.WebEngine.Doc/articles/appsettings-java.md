# Test Configuration File

To simplify command line arguments, some parameters can be specified in a configuration file `application.yml`. 
If a parameter is provided in both side, the value provided in command-line will be taken account.

```yaml
webengineConfiguration:
  name: myproject-automation
  platformName: WINDOWS
  browserName: CHROME
  browserOptionList:
    - --incognito
    - --remote-allow-origins=*
  appiumConfiguration:
    gridConnection: https://hub-cloud.browserstack.com/wd/hub
    userName: XXXXXXX
    password: XXXXXXX
    localTesting:
      activate: false
      arguments:
        force: true
        forcelocal: true
        binarypath: C:\\BrowserStack\\BrowserStackLocal.exe
        localIdentifier: XXXX-YYYY
    capabilities:
      desiredCapabilitiesMap:
        geoLocation: FR
        deviceName: Samsung Galaxy S20 Ultra
        osVersion: 10.0
        projectName: myproject-automation
        buildName: myproject-automation-mobile
        sessionName: Samsung
        local: true
        networkLogs: true
        localIdentifier: XXXX-YYYY
applicationConfiguration:
  values:
    key-1: value-1
    key-2: value-2
    key-3: value-3
```

## General Options
* `name`: The automation project name.
* `platformName`: WINDOWS, ANDROID OR IOS 
* `browserName`: CHROMIUM_EDGE, CHROME, FIREFOX, SAFARI.
* `browserOptionList`: Browser option list
* `appiumConfiguration`: Configuration properties for browserstack. Refer to the browserstack documentation
* `localTesting.activate`: Activate or not local testing
* `Capabilities`: To provided necessary Appium capabilities or options for special need.

> [!NOTE]
> Modifying capabilities may affect the behavior for Web Mobile and App Mobile testing, use it only when it's necessary.
