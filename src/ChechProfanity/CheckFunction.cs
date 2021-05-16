using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using ProfanityList.Check;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CheckProfanityAwsLambda
{
    public class CheckFunction
    {
        /// <summary> Profanity words checker </summary>
        public APIGatewayProxyResponse CheckProfanityHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var profanityList = new ProfanityListS3BucketService();
                var service = new CheckProfanityService(profanityList);

                var exectionDetail = EnumExecuteDetail.CountResult;
                if (request.MultiValueQueryStringParameters != null
                        && request.MultiValueQueryStringParameters.ContainsKey("exectionDetail")
                )
                {
                    var executionDetailStr = request
                        .MultiValueQueryStringParameters["exectionDetail"]
                        .FirstOrDefault();
                    if (Enum.TryParse(executionDetailStr,
                        out EnumExecuteDetail val))
                    {
                        exectionDetail = val;
                    }
                    else
                    {
                        throw new ParseRequestException($"Can't parse ExecuteDetail '{executionDetailStr}'");
                    }
                }

                if (string.IsNullOrEmpty(request.Body))
                    throw new ParseRequestException("Request is empty");

                var buf = Encoding.UTF8.GetBytes(request.Body);
                var ms = new MemoryStream(buf);

                var result = service.CheckProfanity(ms, exectionDetail);

                return new APIGatewayProxyResponse
                {
                    StatusCode = result.ResultStatus == EnumResultStatus.TextIsOk
                                 || result.ResultStatus == EnumResultStatus.TextHasProfanity
                        ? (int)HttpStatusCode.OK
                        : (int)HttpStatusCode.InternalServerError,
                    Body = JsonConvert.SerializeObject(result)
                };
            }
            catch (ParseRequestException e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.BadRequest,
                    Body = e.Message
                };
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = e.Message
                };
            }

        }

        public APIGatewayProxyResponse HealthHandle(ILambdaContext context)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "OK"
            };
        }
    }
}
