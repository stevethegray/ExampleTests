using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace StateInfoSearch;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandlerAsync(string state, ILambdaContext context)
    {
        var states = await InformationSearch.GetAllStateInformationAsync();

        string? abbreviation = null;
        foreach (var stateInfo in states)
        {
            //Converting everything to upper case to make the search case insensitive
            if(stateInfo.state!.ToUpper().Contains(state.ToUpper()))
            {
                abbreviation = stateInfo.abbreviation!;
                break;
            }
        }

        return abbreviation!.ToUpper();
    }
}
