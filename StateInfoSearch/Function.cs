using Amazon.Lambda.Core;
using static StateInfoSearch.InformationSearch;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StateInfoSearch;

public class Function
{
    
    /// <summary>
    /// Searches state information for abbreviations and capitals, input can be a full
    /// or partial state name
    /// </summary>
    /// <param name="stateName">Name of state to search for</param>
    /// <param name="context">Lambda Context</param>
    /// <returns>List containing matching results</returns>
    public async Task<List<StateInfo>> FunctionHandlerAsync(string stateName, ILambdaContext context)
    {
        return await InformationSearch.SearchForStateInformationByNameAsync(stateName);
    }
}
