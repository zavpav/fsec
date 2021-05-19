using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using CheckProfanityAwsLambda;
using ProfanityList.Check;


namespace CheckProfanity.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async void TestHasProfanityVerbosityResult()
        {
            var context = new TestLambdaContext();
            var profanityList = new ProfanityListServiceStub(new List<string> { "ah", "oh", "eh" });
            var function = new CheckProfanityService(profanityList);

            var errText = "ah oh uh oh";
            var stream = this.GetStream(errText);

            var res = await function.CheckProfanity(stream, EnumExecuteDetail.Verbosity);
            
            Assert.Equal(EnumResultStatus.TextHasProfanity, res.ResultStatus);
            Assert.Equal(3, res.CheckingLog.Count);
        }

        [Fact] 
        public async void TestHasProfanityDetailResult()
        {

            var context = new TestLambdaContext();
            var profanityList = new ProfanityListServiceStub(new List<string> { "ah", "oh", "eh" });
            var function = new CheckProfanityService(profanityList);

            var errText = "ah oh uh oh";
            var stream = this.GetStream(errText);

            var res = await function.CheckProfanity(stream, EnumExecuteDetail.Detailed);

            Assert.Equal(EnumResultStatus.TextHasProfanity, res.ResultStatus);
            Assert.Equal(3, res.ProfanityMessagesCount);
            Assert.Equal(2, res.CheckingLog.Count);
            Assert.Equal(2, res.CheckingLog.Single(x => x.Profanity == "oh").Count);
            Assert.Equal(1, res.CheckingLog.Single(x => x.Profanity == "ah").Count);
        }


        private Stream GetStream(string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str, Encoding.UTF8);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        [Fact]
        public async void TestGetMethod()
        {
            TestLambdaContext context;
            APIGatewayProxyResponse response;

            CheckFunction functions = new CheckFunction();

            context = new TestLambdaContext();


            //APIGatewayProxyRequest request;
            //request = new APIGatewayProxyRequest();
            //            response = functions.HealthHandle(request, context);
            response = await functions.HealthHandle(context);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("Hello AWS Serverless", response.Body);
        }

        [Fact]
        public async void TestEmptyBucket()
        {
            IAmazonS3 s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

            //var bucketName = "profanity.storage";
            //await s3Client.PutBucketAsync(bucketName);

            var s3Bucket = new ProfanityListS3BucketService(s3Client);
            await s3Bucket.Add("asdf");

        }

    }
}
