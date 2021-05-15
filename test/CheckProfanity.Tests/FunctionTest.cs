using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;


namespace CheckProfanity.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestHasProfanityVerbosityResult()
        {
            var context = new TestLambdaContext();
            var profanityList = new ProfanityListServiceStub(new List<string> { "ah", "oh", "eh" });
            var function = new Function(profanityList);

            var errText = "ah oh uh oh";
            var stream = this.GetStream(errText);

            var res = function.CheckProfanityHandler(stream, EnumExecuteDetail.Verbosity, context);
            
            Assert.Equal(EnumResultStatus.TextHasProfanity, res.ResultStatus);
            Assert.Equal(3, res.CheckingLog.Count);
        }

        [Fact] public void TestHasProfanityDetailResult()
        {

            var context = new TestLambdaContext();
            var profanityList = new ProfanityListServiceStub(new List<string> { "ah", "oh", "eh" });
            var function = new Function(profanityList);

            var errText = "ah oh uh oh";
            var stream = this.GetStream(errText);

            var res = function.CheckProfanityHandler(stream, EnumExecuteDetail.Detailed, context);

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
