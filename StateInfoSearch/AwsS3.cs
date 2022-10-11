using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StateInfoSearch
{
    /// <summary>
    /// A collection of helper methods for reading data from objects in S3
    /// </summary>
    public static class AwsS3
    {
        //Getting S3 bucket and key information from lambda environment variables
        static string? bucketName = Environment.GetEnvironmentVariable("S3_BUCKET");
        static string? stateInfoKeyName = Environment.GetEnvironmentVariable("S3_KEY");
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;

        //Creating the S3 client that will be re-used throughout this class
        private static IAmazonS3 client = new AmazonS3Client(bucketRegion);

        /// <summary>
        /// Reads data from an S3 object and returns it as a string
        /// </summary>
        /// <param name="keyName">Object key to read</param>
        /// <returns>String contents of S3 object</returns>
        public static async Task<string> ReadObjectDataAsync(string? keyName = null)
        {
            //This example only actually needs one key name, but I am writing the function in
            //such a way as it can return any object
            if(keyName == null)
            {
                keyName = stateInfoKeyName;
            }
            
            string responseBody = "";
            try
            {
                //Executing the request to get the object from the S3 bucket
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                var response = await client.GetObjectAsync(request);

                //Reading the data from the response
                StreamReader reader = new StreamReader(response.ResponseStream);
                responseBody = reader.ReadToEnd();
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                // If any other error was encountered
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }

            //Returning the now fully read response body assuming there were no exceptions thrown
            return responseBody;
        }

    }
}

