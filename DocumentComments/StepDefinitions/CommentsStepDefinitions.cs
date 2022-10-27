using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections;

namespace DocumentComments.StepDefinitions
{
    [Binding]
    public sealed class CommentsStepDefinitions
    {

        private readonly ScenarioContext _scenarioContext;
        IWebDriver _webDriver;

        public CommentsStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _webDriver = new ChromeDriver("D:\\3rdparty\\chrome");
            _webDriver.Url = "https://my.documents.page";
        }

        private string UploadDocument()
        {
            //Here we would write a helper method for uploading a document to the system
            //That would return the necessary ID used to identify the document 
            //This would be best done using API calls if its available
            return "myDocumentId";
        }

        [Given(@"A document is uploaded to the system")]
        public void GivenADocumentIsUploadedToTheSystem()
        {
            _scenarioContext["documentId"] = UploadDocument();
        }

        [Given(@"I am viewing a document")]
        public void GivenIAmViewingADocument()
        {
            //Assuming something like the document id could be used in the URL
            _webDriver.Navigate().GoToUrl($"/{_scenarioContext.Get<string>("documentId")}");
        }

        [When(@"I add a comment to the document")]
        public void WhenIAddACommentToTheDocument()
        {
            //Adding the comment we are adding to the scenario context for later validate
            _scenarioContext["comment"] = "Comment being added to a document";

            //Here we would reference another helper method that would contain the routine
            //for adding comments to the system
        }

        [Then(@"The comment should be displayed")]
        public void ThenTheCommentShouldBeDisplayed()
        {
            //Assuming something like the document id could be used in the URL, and we could navigate
            //from there to the comments
            _webDriver.Navigate().GoToUrl($"/{_scenarioContext.Get<string>("documentId")}/comments");

            IWebElement comment = _webDriver.FindElement(By.XPath("xpath of Webelement"));
            var displayedComment = comment.GetAttribute("value");
            Assert.AreEqual(_scenarioContext.Get<string>("comment"), displayedComment, 
                "Displayed comment value did not match the comment that was entered.");
        }

        [Then(@"The timestamp on the comment should match the current date")]
        public void ThenTheTimestampOnTheCommentShouldMatchTheCurrentDate()
        {
            //Getting the web element that contains the timestamp
            IWebElement timeStamp = _webDriver.FindElement(By.XPath("xpath of Webelement"));
            //Making some assumptions here, such as the format of the timestamp is in a standard format
            //if it is not the Parse command here can be supplemented with a format
            var displayedTimestamp = DateTime.Parse(timeStamp.GetAttribute("value"));

            //More logic would need added here if we were going deeper than day precision
            Assert.AreEqual(DateTime.Today, displayedTimestamp, "Timestamp did not match expected value.");
        }

        [Given(@"The document has more than (.*) comments")]
        public void GivenTheDocumentHasMoreThanComments(int numComments)
        {
            //Here I would write a helper method that would likely use api 
            //routes to add the amount of comments that were needed to the document
            //I would not advise using the UI automation to try to add over 1000 comments
            //That would take too much time and would be prone to failure
        }

        [Given(@"My language is ""([^""]*)""")]
        public void GivenMyLanguageIs(string language)
        {
            //Adding the provided language to the scenario context so it can be validated below
            _scenarioContext["language"] = language;
        }

        [When(@"I open the reiview tab")]
        public void WhenIOpenTheReiviewTab()
        {
            IWebElement reviewTab = _webDriver.FindElement(By.XPath("xpath of Webelement"));
            reviewTab.Click();
            //would need to put in some validation here to make sure the correct tab displays
        }

        [Then(@"A thousands separator of ""([^""]*)"" should be used")]
        public void ThenAThousandsSeparatorOfShouldBeUsed(string separator)
        {
            Hashtable separators = new Hashtable();
            separators.Add("English", ",");
            separators.Add("Italian", ".");

            var currentLanguage = _scenarioContext.Get<string>("language");

            //Example of getting the value of a web element on a page, in this case we are looking for
            //that thousands separator on the comment count
            IWebElement numberofComments = _webDriver.FindElement(By.XPath("xpath of Webelement"));
            Assert.IsTrue(numberofComments.Displayed, "Unable to find number of comments value displayed on the page");
            //Making some assumptions here such as the attribute containing the value is named "value"
            var displayedValue = numberofComments.GetAttribute("value");

            //Making sure the element has the values we are checking for
            Assert.IsTrue(int.Parse(displayedValue) > 1000, "Number of comments was not greater than 1000");
            Assert.IsTrue(numberofComments.GetAttribute("value").Contains(separator), 
                $"The expected separator of {separator} was not found in the value.");
            Assert.AreEqual(separators[currentLanguage], displayedValue[1],
                $"The displayed separator did not match the expected separator for {currentLanguage}");
        }

    }
}