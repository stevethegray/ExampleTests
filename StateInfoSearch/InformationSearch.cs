using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
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

        /// <summary>
        /// Search for state abbreviation and capital information based on the state name,
        /// returns results for exact or partial matches
        /// </summary>
        /// <param name="stateName">Full or partial state name to search for</param>
        /// <returns>List containing abbreviation and capital info for matched results</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<List<StateInfo>> SearchForStateInformationByNameAsync(string stateName)
        {
            //Getting all of the available state information to search through
            var allStateInfo = await GetAllStateInformationAsync();
            
            //Creating a new list that will contain all of our search matches
            List<StateInfo> stateMatches = new List<StateInfo>();

            //Checking each entry in allStateInfo for full or partial matches
            //and adding that information to the return object
            foreach (var stateInfo in allStateInfo)
            {
                //Converting everything to upper case to make the search case insensitive
                if (stateInfo.state!.ToUpper().Contains(stateName.ToUpper()))
                {
                    //Adding all full or partial matches to the list that will be returned.
                    stateMatches.Add(stateInfo);
                }
            }

            //Throwing an exception if no matches were found
            if (stateMatches.Count.Equals(0))
            {
                throw new Exception($"No State information was found matching input of {stateName}");
            }

            return stateMatches;
        }
    }
}
