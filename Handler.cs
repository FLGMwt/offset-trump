using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AwsDotnetCsharp
{
    public class Handler
    {
        public Response GetPledges(Request request)
        {
            return new Response($"Go Serverless v1.0! Your function executed successfully! body: {request.Body}");
        }

        public Response CreatePledge(Request request)
        {
            return new Response($"Go Serverless v1.0! Your function executed successfully! body: {request.Body}");
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
            this.Body = JsonConvert.SerializeObject(body);
        }
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; } = 200;
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
