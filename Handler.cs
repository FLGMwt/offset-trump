using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsDotnetCsharp
{
    public class Handler
    {
        private const string PLEDGE_ID = "pledgeID";
        private const string TABLE_NAME = "offset-trump";

        public async Task<Response> GetPledges(Request request)
        {
            var client = new AmazonDynamoDBClient();
            var response = await client.ScanAsync(TABLE_NAME, new List<string> { PLEDGE_ID });
            var pledges = response.Items.Select(item => new { Pledge = item[PLEDGE_ID].S }).ToList();
            return new Response(pledges);
        }

        public async Task<Response> CreatePledge(Request request)
        {
            var pledge = ExtractPledge(request.Body);

            var client = new AmazonDynamoDBClient();
            await client.PutItemAsync(new PutItemRequest
            {
                TableName = TABLE_NAME,
                Item = new Dictionary<string, AttributeValue> { { PLEDGE_ID, new AttributeValue(pledge) } }
            });
            return new Response();
        }

        private static string ExtractPledge(string body)
        {
            var extractedFormData = body.Split('\n')[3];
            return extractedFormData.Replace("\"", "");
        }
    }

    public class Request
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class Response
    {
        public Response()
        { }

        public Response(object body)
        {
            Body = JsonConvert.SerializeObject(body);
        }
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; } = 200;
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }
            = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*"}};
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
