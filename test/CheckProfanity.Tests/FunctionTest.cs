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
            var profanityList = new ProfanityListServiceStub(new List<string> { "ah", "oh", "eh" });
            var function = new CheckProfanityService(profanityList, null);

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
            var function = new CheckProfanityService(profanityList, null);

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
    }
}
