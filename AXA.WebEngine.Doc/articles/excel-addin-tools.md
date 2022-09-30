# Tools integrated in the Excel Add-in
## Code generation
This function generates the code of the Parameter List in C# and Java, based on the data from sheet `PARAMS`. let you use the property name instead of `string` for test data.
The code is copied to the clipboard, paste it in your IDE to update the class definition.

As a result, with the example excel test data file. it will generate codes for `ParameterList` class as follow. You can paste the code in your data and use them in the test scripts.
In test script, you can call `ParameterList.LASTNAME` to refer the name of the parameter or `GetParameter(ParameterList.LASTNAME)` to get its value.

> [!TIP]
> Use ParameterList class instead of parameters stored in `string`
> can avoid typographical errors and it contributes on the maintainability of the test project.

# [.NET](#tab/netcore)
```C#
public static class ParameterList {

	///<Summary>Parameter: Le nom de cas de test. Il doit être unique dans chaque test suite (dont une feuille Excel) </Summary>
	public static string TESTCASE {get; } = "TESTCASE";

	///<Summary>Parameter: Workflow of the current test case, decides which script will be used to test current scenario. SIMPLE for basic named based search, ADVANCED for multiple filter based search </Summary>
	public static string WORKFLOW {get; } = "WORKFLOW";

	///<Summary>Parameter: The test Environment. Possible values: DEV, TEST, STAGING </Summary>
	public static string ENVIRONMENT {get; } = "ENVIRONMENT";

	///<Summary>Parameter: Search_a_user </Summary>
	public static string Search_a_user {get; } = "Search_a_user";

	///<Summary>Parameter: Last name of the person, used in SIMPLE MODE </Summary>
	public static string LASTNAME {get; } = "LASTNAME";

	///<Summary>Parameter: First name of the person, used in SIMPLE MODE </Summary>
	public static string FIRSTNAME {get; } = "FIRSTNAME";

	///<Summary>Parameter: Agent Number, used in ADVANCED search mode </Summary>
	public static string AGENT_NO {get; } = "AGENT_NO";

	///<Summary>Parameter: Verifications </Summary>
	public static string Verifications {get; } = "Verifications";

	///<Summary>Parameter: Y/N, if the test script will click on "See Details" and check expected values </Summary>
	public static string SEE_DETAILS {get; } = "SEE_DETAILS";

	///<Summary>Parameter: Verify the company field in search result, empty value = ignore the check. </Summary>
	public static string COMPANY {get; } = "COMPANY";

	///<Summary>Parameter: Verify the email field in search result, empty value = ignore the check. </Summary>
	public static string EMAIL {get; } = "EMAIL";

	///<Summary>Parameter: Verify the phone number in search result. </Summary>
	public static string TELEPHONE {get; } = "TELEPHONE";

	///<Summary>Parameter: Verify the city name in search result. </Summary>
	public static string CITY {get; } = "CITY";

}
```
# [Java](#tab/java)
```java
public class ParameterList {
	/** Parameter: Le nom de cas de test. Il doit être unique dans chaque test suite (dont une feuille Excel) */
	public static final String TESTCASE = "TESTCASE";

	/** Parameter: Workflow of the current test case, decides which script will be used to test current scenario. SIMPLE for basic named based search, ADVANCED for multiple-filter based search */
	public static final String WORKFLOW = "WORKFLOW";

	/** Parameter: The test Environment. Possible values: DEV, TEST, STAGING */
	public static final String ENVIRONMENT = "ENVIRONMENT";

	/** Parameter: Search_a_user */
	public static final String Search_a_user = "Search_a_user";

	/** Parameter: Last name of the person, used in SIMPLE MODE */
	public static final String LASTNAME = "LASTNAME";

	/** Parameter: First name of the person, used in SIMPLE MODE */
	public static final String FIRSTNAME = "FIRSTNAME";

	/** Parameter: Agent Number, used in ADVANCED search mode */
	public static final String AGENT_NO = "AGENT_NO";

	/** Parameter: Verifications */
	public static final String Verifications = "Verifications";

	/** Parameter: Y/N, if the test script will click on "See Details" and check expected values */
	public static final String SEE_DETAILS = "SEE_DETAILS";

	/** Parameter: Verify the company field in search result, empty value = ignore the check. */
	public static final String COMPANY = "COMPANY";

	/** Parameter: Verify the email field in search result, empty value = ignore the check. */
	public static final String EMAIL = "EMAIL";

	/** Parameter: Verify the phone number in search result. */
	public static final String TELEPHONE = "TELEPHONE";

	/** Parameter: Verify the city name in search result. */
	public static final String CITY = "CITY";

}
```
***

## Test Data Verification
Test data verification tool checks the test data parameter used in your current spreadsheet versus the parameters in PARAM sheet.

If you are using an unreferenced parameter, or the parameter registered in PARAM sheet but not present in your test data, The tool will warn you about this incoherence.

This tool is used when you have multiple sheets, where you need to add remove or rename parameters along with the evolution of automated tests.

## Test Data Synchronization
Test data synchronization (or Data Merge) tool is similar to test data verification. but automatically adds missing parameters to your current data sheet.

It is useful when you added parameters in PARAM sheet and add them to test data sheet.
