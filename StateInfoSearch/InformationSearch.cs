using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StateInfoSearch
{
    /// <summary>
    /// Helper methods and classes for gathering and searching state information from 
    /// the json object retreived from S3 storage
    /// </summary>
    public static class InformationSearch
    {
        /// <summary>
        /// Class to construct the abbreviation and capital information for each state and territory
        /// </summary>
        public class StateInfo
        {
            public string? state { get; set; }
            public string? abbreviation { get; set; }
            public string? capital { get; set; }
        }

        /// <summary>
        /// Gets the state information from S3 and turns it in to a List of StateInfo
        /// objects for processing.
        /// </summary>
        /// <returns>List of StateInfo objects</returns>
        public static async Task<List<StateInfo>> GetAllStateInformationAsync()
        {
            var responseString = await AwsS3.ReadObjectDataAsync();
            return JsonConvert.DeserializeObject<List<StateInfo>>(responseString)!;
        }
    }
}
