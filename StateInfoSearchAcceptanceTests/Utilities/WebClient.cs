using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace StateInfoSearchAcceptanceTests.Utilities
{
    /// <summary>
    /// Client class for executing requests against our example state information search system
    /// </summary>
    public static class WebClient
    {
        
        //In a production scenario this value would be pulled from environment variables or
        //other external settings
        static string URI = "https://tf1hhw0e2a.execute-api.us-east-1.amazonaws.com";

        public static async Task<RestResponse> GetStateInfo(string searchTerm)
        {
            //Creating the new rest client using the base URI we got from the runsettings
            var client = new RestClient(URI);

            //Building the request for the /stateinfosearch endpoint
            var request = new RestRequest("/stateinfosearch");
            //Serializing our object to a string that this particular test route is expecting
            var orderString = JsonConvert.SerializeObject(searchTerm);
            request.AddStringBody(orderString, DataFormat.None);
            var response = await client.ExecutePostAsync(request);

            //Returning the response object
            return response;
        }

        /// <summary>
        /// Response object that is returned from a successful state info search
        /// </summary>
        public class StateInfoSearchResponse
        {
            public string state { get; set; }
            public string abbreviation { get; set; }
            public string capital { get; set; }
        }

        
    }
}