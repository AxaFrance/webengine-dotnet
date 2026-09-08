# Locators (web)

Priority: `Id` > `Name` > `data-testid`/`data-test` (`Attributes`) > `aria-label` (`Attributes`) > `TagName+InnerText` > `TagName+Id/Name` > `LinkText` (`<a>` only) > `ClassName` (stable only) > `CssSelector`/`XPath` (last resort).

| Captured tag | WebElementDescription |
|---|---|
| `<input id="email">` | `Id = "email"` |
| `<input name="phone">` | `Name = "phone"` |
| `<button data-testid="submit">` | `Attributes = [new HtmlAttribute("data-testid","submit")]` |
| `<button>Continue</button>` | `TagName="button", InnerText="Continue"` |
| `<a>Legal notices</a>` | `LinkText = "Legal notices"` |
| `<div aria-label="menu">` | `Attributes = [new HtmlAttribute("aria-label","menu")]` |

Prefer `Ref` from snapshot when acting via MCP; convert resolved `ElementTag` to the above when generating code.
