// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using ProfanityList.WordList;
using Serilog;

namespace CheckProfanityAwsLambda
{
    public class BasketFunctions
    {
        private IProfanityListService Basket { get; }

        public BasketFunctions()
        {
            this.Basket = ProfanityListFactory.GetProfanityListService();
        }

        /// <summary> Endpoint for add word </summary>
        public async Task<APIGatewayProxyResponse> BasketAddWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logger = LambdaLoggerExtension.TryCreateSerilogLogger();
            try
            {
                var result = await InternalExecute(request, this.Basket.Add, logger);
                return result;
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.InternalServerError,
                    Body = "Exception " + e.Message + "\n" + e.ToString()
                };
            }
            finally
            {
                logger.TryDispose();
            }
        }

        /// <summary> Endpoint for remove word </summary>
        public async Task<APIGatewayProxyResponse> BasketRemoveWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logger = LambdaLoggerExtension.TryCreateSerilogLogger();
            try
            {
                var result = await InternalExecute(request, this.Basket.Remove, logger);
                return result;
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = "Exception " + e.Message + "\n" + e.ToString()
                };
            }
            finally
            {
                logger.TryDispose();
            }
        }

        /// <summary> Endpoint for the words list </summary>
        public async Task<APIGatewayProxyResponse> BasketListWordHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logger = LambdaLoggerExtension.TryCreateSerilogLogger();
            try
            {
                this.Basket.SetLogger(logger);

                var list = await this.Basket.GetProfanityWordList();
                var result = string.Join(", ", list);

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.OK,
                    Body = $"<result>{result}</result>"
                };
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int) HttpStatusCode.InternalServerError,
                    Body = "Exception " + e.Message + "\n" + e.ToString()
                };
            }
            finally
            {
                logger?.TryDispose();
            }
        }


        /// <summary> Internal executing (extract parameters and workaround code) </summary>
        private async Task<APIGatewayProxyResponse> InternalExecute(APIGatewayProxyRequest request,
            Func<string, Task<BasketEditResult>> executeFunc, ILogger? logger)
        {
            try
            {
                this.Basket.SetLogger(logger);

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