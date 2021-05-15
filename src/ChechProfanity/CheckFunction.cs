using System.IO;
using Amazon.Lambda.Core;
using ProfanityList.Check;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CheckProfanityAwsLambda
{
    public class CheckFunction
    {
        /// <summary> Profanity words checker </summary>
        /// <param name="inputStream">Income text in UTF-8 encoding</param>
        /// <param name="exectionDetail">Required execution detalization</param>
        /// <param name="context">AWS context</param>
        /// <returns>Information about checking the inputStream text</returns>
        public CheckProfanityResult CheckProfanityHandler(Stream inputStream, EnumExecuteDetail exectionDetail, ILambdaContext context)
        {
            var profanityList = new ProfanityListS3BucketService();
            var service = new CheckProfanityService(profanityList);

            return service.CheckProfanity(inputStream, exectionDetail);
        }

    }
}
