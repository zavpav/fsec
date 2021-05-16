// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

using System;
using System.Net;
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
        public APIGatewayProxyResponse BasketAddWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return InternalExecute(request, this.Basket.Add);
        }

        /// <summary> Endpoint for remove word </summary>
        public APIGatewayProxyResponse BasketRemoveWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return InternalExecute(request, this.Basket.Remove);
        }

        /// <summary> Internal executing (extract parameters and workaround code) </summary>
        private APIGatewayProxyResponse InternalExecute(APIGatewayProxyRequest request,
            Func<string, BasketEditResult> executeFunc)
        {
            if (request.PathParameters != null
                && request.PathParameters.ContainsKey("word")
            )
            {
                var word = request.PathParameters["word"];
                var result = executeFunc(word);

                return new APIGatewayProxyResponse
                {
                    StatusCode = result.Result == BasketEditResult.EnumResult.Ok
                        ? (int) HttpStatusCode.OK
                        : (int) HttpStatusCode.InternalServerError,
                    Body = JsonConvert.SerializeObject(result)
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.InternalServerError,
                Body = "'word' path not found"
            };
        }
    }
}