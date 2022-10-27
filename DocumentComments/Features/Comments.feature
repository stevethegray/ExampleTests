Feature: Comments

As a user I want to be able to add comments to documents and see the count of comments
on a document with a language specific thousands seperator

Background:

# this will act as a setup scenario before the tests run, in this case uploading a document
# so the tests can assume a document exists and uploading a document does not need to be explicitly stated
# in each test
Given A document is uploaded to the system

@sucessScenarios
Scenario: When a new comment is created the timestamp is correct
	Given I am viewing a document
	When I add a comment to the document
	Then The comment should be displayed
		And The timestamp on the comment should match the current date

Scenario Outline: When comment counts exceen 1000 a language appropriate thousands separator appears
	Given I am viewing a document
		And The document has more than 1000 comments
		And My language is "<language>"
	When I open the reiview tab
	Then A thousands separator of "<separator>" should be used

	Examples: 
	| language | separator |
	| English  | ,         |
	| Italian  | .         |