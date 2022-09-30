---
uid: AXA.WebEngine.Report
summary: *content
---

The AXA.WebEngine.Repport namespace contains classes and interfaces for the generation of test reports. You can use ReportHelper to generate other compatible test reports such as JUnit Test Report.


In general, the structure of a report is following:
```
TestSuiteReport           <- Report for the whole test suite
|
+ EnvironmentVariables    <- EnvironmentVariables used during the test
+ TestReport              <- Report of individual Test Cases
  |
  + TestData              <- Test data used during the execution
  + ActionReports         <- Report of Actions with-in the test case
    |
    + ContextValues       <- Context variables generated in this action
    + Screenshots         <- The screenshots generated in this action
    + SubActionReports    <- Report of Sub-actions with-in the action
```
