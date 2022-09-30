Feature: DrinkMachine

@tag1
Scenario: Let's drink some tea
	Given I turn on the drink machine
	When I select 'Français' as language and order a 'Tea'
	Then I got 'Tea'
