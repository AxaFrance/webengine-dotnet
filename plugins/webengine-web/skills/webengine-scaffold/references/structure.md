# Structure + test data (scaffold)

## Keyword-Driven layout (inside project folder)

```
PageModels/  <- *ElementDescription + PageModel
Actions/     <- SharedActionWeb / SharedActionApp (DoAction + DoCheckpoint)
TestCases/   <- TestCaseWeb / TestCaseApp with TestSteps[]
TestData/    <- XML datasets
ParameterList.cs <- string constants, use GetParameter(ParameterList.X)
```

TestCase:
```csharp
[Description("Car insurance quote")]
public class TC_InsuranceQuote : TestCaseWeb {
  public TC_InsuranceQuote() {
    TestSteps = new TestStep[] {
      new() { Action = nameof(Login) },
      new() { Action = nameof(ValidateQuote) } };
  }
}
```

Test data XML (`http://www.axa.fr/WebEngine/2022`):
```xml
<TestSuiteData xmlns="http://www.axa.fr/WebEngine/2022">
  <TestData><TestName>Devis_Auto_Standard</TestName>
    <Data>
      <Variable><Name>TESTCASE</Name><Value>Devis_Auto_Standard</Value></Variable>
      <Variable><Name>URL</Name><Value>https://www.example.com/devis</Value></Variable>
    </Data></TestData>
</TestSuiteData>
```

ParameterList:
```csharp
public static class ParameterList {
  /// <summary>Target environment URL</summary>
  public static string URL { get; } = "URL";
}
```
