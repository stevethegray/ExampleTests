using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using StateInfoSearchAcceptanceTests.Utilities;
using System.Net;
using static StateInfoSearchAcceptanceTests.Utilities.WebClient;

namespace StateInfoSearchAcceptanceTests.StepDefinitions
{
    [Binding]
    public sealed class StateInfoSearchStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        public StateInfoSearchStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I want to search for states that start with ""([^""]*)""")]
        [Given(@"I want to search for the state ""([^""]*)""")]
        public void GivenIWantToSearchForStatesThatStartWith(string searchTerm)
        {
            Assert.IsNotNull(searchTerm, "Must provide a value for search term.");
            _scenarioContext["searchTerm"] = searchTerm;
        }

        [When(@"I execute the search")]
        public async Task WhenIExecuteTheSearchAsync()
        {
            var webResponse = await GetStateInfo(_scenarioContext.Get<string>("searchTerm"));            
            _scenarioContext.Add("webResponse", webResponse);
        }

        //Helper method that takes the web response and converts it to the StateInfoSearchResponse object
        //After checking for errors
        public List<StateInfoSearchResponse> GetStateResponse(RestResponse webResponse)
        {
            Assert.AreEqual(HttpStatusCode.OK, webResponse.StatusCode, $"Response containted error code ${webResponse.StatusCode} - ${webResponse.ErrorMessage}");
            return JsonConvert.DeserializeObject<List<StateInfoSearchResponse>>(webResponse.Content);
        }

        [Then(@"I should see ""([^""]*)"" among the results")]
        public void ThenIShouldSeeAmongTheResults(string expectedStateName)
        {
            bool expectedResultFound = false;
            foreach(var info in GetStateResponse(_scenarioContext.Get<RestResponse>("webResponse")))
            {
                if (info.state.ToUpper().Equals(expectedStateName.ToUpper()))
                {
                    expectedResultFound = true;
                }
            }

            Assert.IsTrue(expectedResultFound, "Expected state name was not found among the returned results.");
        }

        [Then(@"Each resulting state name should contain the ""([^""]*)""")]
        public void ThenEachResultingStateNameShouldContainThe(string searchTerm)
        {
            Assert.IsNotNull(searchTerm, "Must provide a value for search term.");
            _scenarioContext["searchTerm"] = searchTerm;

            GetStateResponse(_scenarioContext.Get<RestResponse>("webResponse")).ForEach(info => 
                Assert.IsTrue(info.state.ToUpper().Contains(searchTerm.ToUpper()), "Returned state name did not contain the search term."));
        }

        [Then(@"The resulting abbreviation should be ""([^""]*)""")]
        public void ThenTheResultingAbbreviationShouldBe(string abbreviation)
        {
            GetStateResponse(_scenarioContext.Get<RestResponse>("webResponse")).ForEach(info =>
                Assert.IsTrue(info.abbreviation.ToUpper().Equals(abbreviation.ToUpper()), "Returned state abbreviation did not match expected result."));
        }

        [Then(@"I should see an error message")]
        public void ThenIShouldSeeAnErrorMessage()
        {
            Assert.AreEqual(HttpStatusCode.BadRequest, _scenarioContext.Get<RestResponse>("webResponse").StatusCode, "Expected an error code to be returned in this scenario.");
            Assert.IsNotNull(_scenarioContext.Get<RestResponse>("webResponse").Content, "Expected an error message to be returned in this scenario.");
        }


    }
}