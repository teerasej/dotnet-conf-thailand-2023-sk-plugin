using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace MathPlugin
{
    public class Add
    {
        private readonly ILogger _logger;

        public Add(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Add>();
        }

        [Function("Add")]
        [OpenApiOperation(operationId: "Add", tags: new[] { "Math" }, Summary = "Add two numbers", Description = "This function adds two numbers.")]
        [OpenApiParameter(name: "number1", In = ParameterLocation.Query, Required = true, Type = typeof(double), Summary = "First number", Description = "The first number to add.")]
        [OpenApiParameter(name: "number2", In = ParameterLocation.Query, Required = true, Type = typeof(double), Summary = "Second number", Description = "The second number to add.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The OK response", Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            bool result1 = double.TryParse(req.Query["number1"], out double number1);
            bool result2 = double.TryParse(req.Query["number2"], out double number2);

            if (result1 && result2)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain");
                double sum = number1 + number2;
                response.WriteString(sum.ToString(CultureInfo.CurrentCulture));

                _logger.LogInformation($"Add function processed a request. Sum: {sum}");

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Please pass two numbers on the query string or in the request body");

                return response;
            }
        }
    }
}