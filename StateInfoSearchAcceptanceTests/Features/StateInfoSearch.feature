Feature: StateInfoSearch
As a web user
I want to search for a whole or partial name of a state
And get back state name, abbreviation, and capital information for that state

@successPath
Scenario Outline: Searching for partial matches
	Given I want to search for states that start with "<searchTerm>"
	When I execute the search
	Then I should see "<stateName>" among the results
		And Each resulting state name should contain the "<searchTerm>"

	Examples:
	| searchTerm | stateName      |
	| west       | West Virginia  |
	| North      | North Carolina |

@successPath
Scenario Outline: Searching for state abbreviations
	Given I want to search for the state "<searchTerm>"
	When I execute the search
	Then I should see "<stateName>" among the results
		And Each resulting state name should contain the "<searchTerm>"
		And The resulting abbreviation should be "<abbreviation>"

	Examples:
	| searchTerm     | stateName      | abbreviation |
	| Pennsylvania   | Pennsylvania   | PA           |
	| North Carolina | North Carolina | NC           |
	
@failurePath
Scenario Outline: Searching for a state that does not exist
	Given I want to search for states that start with "BumbleBeeTuna"
	When I execute the search
	Then I should see an error message