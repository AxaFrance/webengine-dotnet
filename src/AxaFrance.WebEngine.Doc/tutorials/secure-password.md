# Secure Passwords

During the test, you may need test account where a password is used. To protect your test environment, it is recommended to secure the password, that is:
-	Do not use clear password in the code
-	Do not store clear password in test data file
-	If necessary, store encrypted password in test data, and decrypt data when passwords are used.

WebEngine provides a native solution to achieve this:
-	A method to encrypt password
-	A method to securely set password into password box

## Encryption
For the encryption, you can set `encryption key` in [appsetting.json](../articles/appsetting.md), then use any of following methods to calculate encrypted data:
### Via WebRunner
If you have an encryption key stored in appsettng.json (not recommanded in shared storage), you can pass this command line to get encrypted data:
```base
> WebRunner.exe -encrypt helloworld
Encrypted data is: Iw6buAX7oY97dbk/w3gXLA==
```


For security reason, it is recommanded to leave a Token in `appsettings.json` then replace it's value on DevOps platform, or to pass `-encryptionKey` parameter in command line to launch the test. 
```base
> WebRunner.exe -encrypt helloworld -encryptionKey:my128bitkey
Encrypted data is: rTGtyWKevbTjT3RkSAJvYA==
```


### Via `Encrypter.Encrypt(string)`
If you do not use WebRunner (for example, you are using Gherkin or Linear Scripting Approach) you can use <xref:AxaFrance.WebEngine.Encrypter.Encrypt(System.String)> method to calculate encrypted data in the code. In this case, you'll need to put ecryption key in `appsetting.json`

Once the data is encrypted, it can be used in the code or in the test data.

## Use encrypted password
In order not to expose the encrypted data to the user, WebEngine does not make decryption function accessible from outside. To put encrypted data in a password box, you'll need to use the method: 
<xref:AxaFrance.WebEngine.Web.WebElementDescription.SetSecure(System.String)>.

- SetSecure only works on password type input. It takes the encrypted data, decrypts it and fill it on the fly.
- If the field is not a password box, SetSecure refuses to work and will throw an exception
- You can of course use SetValue() or SendKeys() to put a clear password in the password field.

> [!NOTE]
> If you are using a custom key, do not commit `appsettings.json` containing real encryption key.
> Instead on DevOps environment, use token replacing method to valorise securely the encryption key. you can even use different encryption key for different test environments.

## Example

```csharp
public void SecurePassword()
{
    WebElementDescription passwordBox = new WebElementDescription(driver) {
        Id = "password"
    };
    WebElementDescription inputBox = new WebElementDescription(driver)
    {
        Id = "inputValue"
    };
    var password = Encrypter.Encrypt("password");
    passwordBox.SetSecure(password); // -> OK
    inputBox.SetSecure(password);    // -> Error. inputBox is not a password box
}
```