using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace OrderTests
{
    /// <summary>
    /// Client class for executing requests against our example order system
    /// </summary>
    public static class Client
    {
        //It's good practice to keep properties like base URI in a runsettings file
        //You can have multiple runsettings file for hitting different environments
        public static TestContext TestContext { get; set; }
        static string URI = TestContext.Parameters["URI"];

        public static async Task<RestResponse> PlaceOrderAsync(Order order)
        {
            //Creating the new rest client using the base URI we got from the runsettings
            var client = new RestClient(URI);
            //Any authentication the endpoint might have would be handled here

            //Building the request for the /order endpoint
            var request = new RestRequest("/order");
            //Serializing our object to a string that this particular test route is expecting
            var orderString = JsonConvert.SerializeObject(order);
            request.AddStringBody(orderString, DataFormat.None);
            var response = await client.ExecutePostAsync(request);

            //Throwing an http exception if a status code 400 was encountere
            //We will use this for validation in the tests we write
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new HttpRequestException(statusCode: response.StatusCode, message: response.Content, inner: null);
            }
            
            //Returning the success response if we got this far
            return response;
        }

        /// <summary>
        /// Using a data model for the order request, its easier to manipulate a data model
        /// than it is to work with strings.  I am allowing all properties to be nullable so I can purposely
        /// make bad requests to check the validation messages from the api
        /// </summary>
        public class Order
        {
            public string OrderId { get; set; }
            public string OrderName { get; set; }
            public string CustomerAccount { get; set; }
            public string PaymentTerms { get; set; }
            public bool? TaxExempt { get; set; }
            public string Currency { get; set; }
            public Address BillToPerson { get; set; }
            public Address ShipToPerson { get; set; }

            public class Address
            {
                public string EmailAddress { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string StreetLine1 { get; set; }
                public string StreetLine2 { get; set; }
                public string StreetLine3 { get; set; }
                public string City { get; set; }
                public string Country { get; set; }
                public string Region { get; set; }
            }
        }

        /// <summary>
        /// Helper method for geting a valid order object to start with
        /// will use this to verify a correct data model and then
        /// manipulate it to create invalid requests so we can validate error messages
        /// </summary>
        /// <returns>Order object</returns>
        public static Order GetValidOrder()
        {
            Order validOrder = new Order();

            validOrder.OrderId = "123456";
            validOrder.OrderName = "1 Year - Minitab Statistical Software";
            validOrder.CustomerAccount = "Bedrock, LLC";
            validOrder.PaymentTerms = "NET30";
            validOrder.TaxExempt = false;
            validOrder.Currency = "USD";
            validOrder.BillToPerson = GetValidAddress();
            validOrder.ShipToPerson = GetValidAddress();

            return validOrder;
        }

        /// <summary>
        /// Gets a valid Address object that is used inside the Order object
        /// </summary>
        /// <returns>Address object</returns>
        public static Order.Address GetValidAddress() 
        {
            Order.Address validAddress = new Order.Address();

            validAddress.EmailAddress = "fred.flintstone@bedrock.com";
            validAddress.FirstName = "Fred";
            validAddress.LastName = "FlintStone";
            validAddress.StreetLine1 = "1829 Pine Hall Rd.";
            validAddress.City = "State College";
            validAddress.Region = "PA";
            validAddress.Country = "US";

            return validAddress;
        }
    }
}
