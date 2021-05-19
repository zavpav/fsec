// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class BasketFunctions
    {
        private ProfanityListS3BucketService Basket { get; }

        public BasketFunctions()
        {
            this.Basket = new ProfanityListS3BucketService();
        }
        
        /// <summary> Endpoint for add word </summary>
        public async Task<APIGatewayProxyResponse> BasketAddWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = await InternalExecute(request, this.Basket.Add);
            return result;
        }

        /// <summary> Endpoint for remove word </summary>
        public async Task<APIGatewayProxyResponse> BasketRemoveWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = await InternalExecute(request, this.Basket.Remove);
            return result;
        }

        /// <summary> Endpoint for the words list </summary>
        public async Task<APIGatewayProxyResponse> BasketListWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var list = await this.Basket.GetProfanityWordList();
            var result = string.Join(", ", list);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = $"<result>{result}</result>"
            };
        }


        /// <summary> Internal executing (extract parameters and workaround code) </summary>
        private async Task<APIGatewayProxyResponse> InternalExecute(APIGatewayProxyRequest request,
                        Func<string, Task<BasketEditResult>> executeFunc)
        {
            try
            {
                if (request.PathParameters != null
                    && request.PathParameters.ContainsKey("word")
                )
                {
                    var word = request.PathParameters["word"];
                    var result = await executeFunc(word);

                    return new APIGatewayProxyResponse
                    {
                        StatusCode = result.Result == BasketEditResult.EnumResult.Ok
                            ? (int)HttpStatusCode.OK
                            : (int)HttpStatusCode.InternalServerError,
                        Body = JsonConvert.SerializeObject(result)
                    };
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = "'word' path not found"
                };
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = "Exception " + e.Message + "\n" + e.ToString()
                };
            }
        }
    }
}