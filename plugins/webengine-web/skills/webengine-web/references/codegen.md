# Codegen from ElementTag (web)

Use the `Element tag: <...>` returned by each action — never guess.

## Playwright + TypeScript

| Tag | Playwright |
|---|---|
| `id="submitBtn"` | `page.getByTestId` if `data-testid`, else `page.locator('#submitBtn')` |
| `aria-label="Validate"` | `page.getByRole('button', { name: 'Validate' })` |
| `<button>Continue</button>` | `page.getByRole('button', { name: 'Continue' })` |
| `name="email"` | `page.locator('input[name="email"]')` |

Example:
```ts
await page.goto('https://example.com/login');
await page.locator('#email').fill('test@example.com');
await page.getByRole('button', { name: 'Sign in' }).click();
```

## Selenium Java / Python

Map `Id`→`By.id`, `Name`→`By.name`, test-id→`By.cssSelector("[data-testid='x']")`, text→XPath `//button[normalize-space()='Continue']` (last resort).

## WebEngine C# — see webengine.md.
