# Data-Driven testing Approach (Java/Spring boot)

This article will show steps to build an Automation solution based on Data-Driven testing approach with `Externalized test data` and `Dynamic execution based on test data`.
We will continue from the previous test project for application http://webengine-test.azurewebsites.net/home-insurance.

# Prerequisite:
* Familiar with [Keyword-Driven testing approach](keyworddriven.md)
* Have already followed the tutorial [Keyword-Driven testing Approach (Java/Spring boot)](keyword-driven-java.md) and have a working project.

# Step 1: Review modulization, project structure and keywords
The modelling of Home Insurance Underwriting application may look like following diagram:

![Schemas](../images/kd-schemas.png)

> [!NOTE]
> This tutorial is based on the outcome of [Keyword-Driven testing Approach (Java/Spring boot)](keyword-driven-java.md). If you don't have a working keyword-driven test project yet, please follow that article first.

# Step 2: Identify variables
In this step, we'll need to identify variables used in the test automation solution.
To record and manage test data, the most convenient way is to use an Excel spreadsheet under WebEngine format.
So you can run all your tests directly from Excel with WebEngine Excel Add-in.

For more information about Excel test data
* [Excel Add-in](../articles/excel-addin.md)
* [Download Excel test data for this tutorial](../files/Data-HomeInsurance.xlsx)

Test data will have 3 sheets:
* `PARAMS`: All the possible parameters of test case and it's description.
* `ENV`: Test environment dependent variables such as URL or the site or the name of the server.
* `TEST_SUITE`: represents the test suite (including a list of test cases)


For this test, we developed following test data
# [PARAMS](#tab/param)
Describes all the test parameters

![](../images/dd-excel-param-java.png)



# [ENV](#tab/env)
Here we lists all test environment dependent variables such as URLs.

If you have more than one test environment, you can list all data with prefix or postfix

![](../images/dd-excelenv-java.png)


# [TEST_SUITE](#tab/testsuite)
In test data we will specify the test cases and test data used for each test case.

For example: our test suite will have 3 test cases, covers Apartment, House and Apartment with previous accident.
From spreadsheet we can clearly see how parameters are used and have an idea about the test coverage of each parameter.
To increase test coverage, we can simply create new columns without the need to modify the test script.

![](../images/dd-exceldata-java.png)
***

> [!NOTE]
> There is no need to develop every parameter and the variable for every test case.
> The solution can be improved with the time by increasing test coverage in width (by increasing test cases) and in depth (by increasing test parameters)

# Step 3: Using test parameters in the script.
## 3.1: Import test parameters from EXCEL
The function of WebEngine Addin -> Tools -> Code Generation can generate a Java class `ParameterList` for you, including all test parameters with their descriptions as comment

![Excel Code Generation](../images/excel-code-generation.png)

# [Why use ParameterList](#tab/why-parameter-list)
Using parameter list are following advantage:
* Benefit from code auto-completion from development tools.
* Understands the meaning of the parameter thanks to the comments.
* Avoid type error in the test script.

# [Content of ParameterList](#tab/content-parameter-list)
You should split after into two classes for maintenability

```java
package fr.axa.automation.parameter;

public class EnvironnementVariable {
    public static final String  URL = "URL";
    public static final String  LOGIN = "LOGIN";
    public static final String  PASSWORD = "PASSWORD";
}
```

```java
package fr.axa.automation.parameter;

public class DataVariable {
    public static final String  PROSPECT_NAME = "Nicolas";
}
```
***

## 3.2: Use variables in the script
In actions, you can use `EnvironnementVariable.URL` contant to retrieve the URL value of an Environment Variable and use `DataVariable.PROSPECT_NAME` constant to retrieve test data.

For Example, compared to the hard-coded login action, now the keyword action `Login` looks like following code snippet:

* We get the parameter `Environment` from test data
* Get `URL` from Environment Variables.
* Fill `username` and `password` with the value from parameter `Username` and `Password`
  This action can do whatever needed according to the test data provided.

# [Login.java (Hard Coded)](#tab/login-hc)
```java
package fr.axa.automation.action;

import fr.axa.automation.model.LoginModel;
import fr.axa.automation.parameter.EnvironnementVariable;
import fr.axa.automation.webengine.core.AbstractActionWebBase;
import lombok.AccessLevel;
import lombok.experimental.FieldDefaults;

@FieldDefaults(level = AccessLevel.PRIVATE)
public class LoginAction extends AbstractActionWebBase {
  LoginModel loginModel;

  public LoginAction() {
  }

  @Override
  public void doAction() throws Exception {   
    loginModel = new LoginModel(getWebDriver());
    getWebDriver().get("http://webengine-test.azurewebsites.net/home-insurance/");
    loginModel.getLogin().sendKeys("login");
    loginModel.getPassword().sendKeys("pwd");
    screenShot();
    loginModel.getButton().click();
    addInformation("First step succeed");
  }

  @Override
  public boolean doCheckpoint() throws Exception {
    return true;
  }
}
```
# [Login.java (Data Driven)](#tab/login-dd)
```java
package fr.axa.automation.action;

import fr.axa.automation.model.LoginModel;
import fr.axa.automation.parameter.EnvironnementVariable;
import fr.axa.automation.webengine.core.AbstractActionWebBase;
import lombok.AccessLevel;
import lombok.experimental.FieldDefaults;

@FieldDefaults(level = AccessLevel.PRIVATE)
public class LoginAction extends AbstractActionWebBase {
  LoginModel loginModel;

  public LoginAction() {
  }

  @Override
  public void doAction() throws Exception {

    String url = getEnvironnementValueWithException(EnvironnementVariable.URL); // URL Define in env.xml file
    String login = getEnvironnementValueWithException(EnvironnementVariable.LOGIN); // URL Define in env.xml file
    String pwd = getEnvironnementValueWithException(EnvironnementVariable.PASSWORD); // URL Define in env.xml file
    loginModel = new LoginModel(getWebDriver());
    getWebDriver().get(url);
    loginModel.getLogin().sendKeys(login);
    loginModel.getPassword().sendKeys(pwd);
    screenShot();
    loginModel.getButton().click();
    addInformation("First step succeed");
  }

  @Override
  public boolean doCheckpoint() throws Exception {
    return true;
  }
}
```
***

# Step 4: Data-Driven test script

Repeat the step 3.2 on every keyword action, you can completely remove hard coded test data.
But sometimes it's not enough, because test procedure may change as test data changes.

## 4.1 Manages test process driven by data
In our application `home type` can be `apartment` or `house`, according to its value.
The forms will be different.

Our script must implement this logic driven by test data data: when the type is `apartment`, the script will fill form for apartments, otherwise the script will fill form for houses.

This logic is implemented in the keyword action `HomeAction`, according to the value of home type, different sub-action will be executed.
package fr.axa.automation.action;

```java
package fr.axa.automation.action;

import fr.axa.automation.model.HomeDetailModel;
import fr.axa.automation.parameter.DataVariable;
import fr.axa.automation.webengine.core.AbstractActionWebBase;
import lombok.AccessLevel;
import lombok.experimental.FieldDefaults;

@FieldDefaults(level = AccessLevel.PRIVATE)
public class homeDetailTypeAppartmentAction extends AbstractActionWebBase {
  HomeDetailModel homeDetailModel;

  public homeDetailTypeAppartmentAction() {
  }

  @Override
  public void doAction() throws Exception {
    String totalFloor = getParameterWithException(DataVariable.TOTAL_FLOOR);
    String floorNumber = getParameterWithException(DataVariable.FLOOR_NUMBER);
    homeDetailModel = new HomeDetailModel(getWebDriver());
    homeDetailModel.getTypeHomeAppartment().click();
    homeDetailModel.getTotalFloor().selectByValue(totalFloor);
    homeDetailModel.getFloorNumber().sendKeys(floorNumber);
    screenShot();
    addInformation("Home subscription step succeed");
  }

  @Override
  public boolean doCheckpoint() throws Exception {
    return true;
  }
}
```


## 4.3 Resume
With above technics, you can control the test process or optional actions by external data.
That means if every keyword action is implemented like above examples,
you can run tests with any combination of data without the need to modify and update the code.

In general, simple controls can be implemented with `if` statement. but when the flow control becomes complex, it is recommended to separate these logics into sub-actions to keep an action at a reasonable complexity.

# Step 5: Test suite driven by data
We have improved every keyword action, and technically we can run test case with any combination of test data.

# [Test Suite ](#tab/ts-code)

```java
package fr.axa.automation.testsuite;

import fr.axa.automation.testcase.SubscriptionTypeAppartmentTestCase;
import fr.axa.automation.webengine.core.AbstractTestSuite;
import fr.axa.automation.webengine.core.ITestCase;

import java.util.AbstractMap;
import java.util.ArrayList;
import java.util.List;

public class TestSuite extends AbstractTestSuite {

    public static final String SUBSCRIPTION_HOME = "SUBSCRIPTION_HOME"; //Test case defined in data.xml

    @Override
    public List<AbstractMap.SimpleEntry<String, ? extends ITestCase>> getTestCaseList() {
        List<AbstractMap.SimpleEntry<String, ? extends ITestCase>> testCaseList = new ArrayList();
        testCaseList.add(new AbstractMap.SimpleEntry<String, ITestCase>(SUBSCRIPTION_HOME,new SubscriptionTypeAppartmentTestCase()));
        return testCaseList;
    }
}
```
***

# Step 6: Export test data
Now go back to Excel test data and export Test Data and Environment Variables.
Save them in the resource directory.

![](../images/export-test-data-and-env.png)

![](../images/java/data-driven/test-suite-and-env.png)


# Step 7: Debug and Execute test cases
Similar to previous article, you'll need to configure the project properties to launch the Application class with appropriate parameters.


![](../images/java/data-driven/run-data-driven.png)


This time, we will provide 2 more parameters:
* `-data:` to provide Test Data
* `-env:` to provide Environment Variables

Now we are good to go.

Launch your project and now we can see the test is running until the test report is showing:

![](../images/java/data-driven/result-data-driven.png)

If error happens in the test script, you can set breakpoint in the code and debug the script line by line.

# Step 8: Run tests directly from EXCEL
It is also possible to run tests directly from EXCEL. The advantage is that you can run one or more tests via selection.
But before launch the test via EXCEL, you must tell WebEngine Add-in where your automate solution is located.

This can be configured in Settings:
`WebEngine` -> `Settings`

![](../images/java/data-driven/run-data-driven-from-excel.png)

* **Export Directory**: The folder where Test data and Environment variables should be exported.
* **WebRunner Directory**: The folder where `webrunner.exe` or `webrunner.jar` is located along with your test solution.
  In general, the output folder of your test projet in `bin\\debug`
* **Test Assembly**: The compiled library contains your test script. By default, its name is your `<project_name>.dll` for C# projet and `<project_name>.jar` for Java.

Once the settings is done, now we can run any and any number of tests directly:
For example: run `TEST_02`:

# [Select Test](#tab/run-step-1)

Select `SUBSCRIPTION_HOME` cell

![](../images/dd-step1-java.png)
# [Launch Test](#tab/run-step-2)

Click `Launch Test`
![Dd Step2](../images/dd-step2.png)

# [Select Browser](#tab/run-step-3)

Choose a desktop browser, for example `Firefox`. Then click `Start`
![Dd Step3](../images/dd-step3.png)

# [Observe Test Execution](#tab/run-step-4)

Now you can see the framework is running Test_02 on Firefox:
![Dd Step4](../images/dd-step4.png)
***

> [!NOTE]
> By running tests directly from Excel, Webrunner is not attached with Visual Studio debugger. So it's not possible to debug the code line by line.
> If you want to debug a particular test case or action, you can use Excel Add-in to export that particular test case, then launch the project with-in Visual Studio.

# Conclusion
Congratulations! You've reached here and have a dynamic data-driven test solution for your application.

Now you can study test coverage and develop other test cases, if necessary, in both directions:
* In Width: to develop additional test cases with new combination of test data.
  This can be done exclusively within Excel without the need to motify the code or the test project.
* In Depth: If we want to do more verifications or to cover more functionalities. we'll need to update appropriate keyword-actions or add new keyword action, then externalize test data for these newly added codes.
