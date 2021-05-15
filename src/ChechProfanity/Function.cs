using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Microsoft.VisualBasic.CompilerServices;
using ProfanityList;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CheckProfanity
{
    public class Function
    {
        private long MAX_FILE_LEN = 10000;

        private IProfanityListService ProfanityListService { get; set; }

        public Function()
        {
            this.ProfanityListService = new ProfanityListS3BucketService();
        }

        public Function(IProfanityListService profanityListService)
        {
            this.ProfanityListService = profanityListService;
        }

        /// <summary> Profanity words checker </summary>
        /// <param name="inputStream">Income text in UTF-8 encoding</param>
        /// <param name="exectionDetail">Required execution detalization</param>
        /// <param name="context">AWS context</param>
        /// <returns>Information about checking the inputStream text</returns>
        public CheckProfanityResult CheckProfanityHandler(Stream inputStream, EnumExecuteDetail exectionDetail, ILambdaContext context)
        {
            var executeSw = Stopwatch.StartNew();
            
            if (inputStream.Length > this.MAX_FILE_LEN)
            {
                return new CheckProfanityResult
                {
                    Decsription = "Text too long",
                    ExecutionTime = executeSw.Elapsed,
                    VerbosityInfo = exectionDetail,
                    ResultStatus = EnumResultStatus.ExecuteError
                };
            }

            if (exectionDetail == EnumExecuteDetail.Default)
                exectionDetail = EnumExecuteDetail.AnyResult;

            var result = new CheckProfanityResult
            {
                VerbosityInfo = exectionDetail
            };
            
            try
            {
                var profanities = this.GetProfanitiesList();
                result.ProfanityMessagesCount = profanities.Count;

                if (exectionDetail == EnumExecuteDetail.AnyResult)
                {
                    if (this.FastCheckInformation(inputStream, profanities))
                        result.ResultStatus = EnumResultStatus.TextHasProfanity;
                    else
                        result.ResultStatus = EnumResultStatus.TextIsOk;
                }
                else
                {
                    var logChecking = this.CheckProfanity(inputStream, profanities, exectionDetail);
                    var totalCount = logChecking.Sum(x => x.Count);

                    if (exectionDetail == EnumExecuteDetail.CountResult)
                    {
                        result.ProfanityMessagesCount = totalCount;
                    }
                    else if (exectionDetail == EnumExecuteDetail.Detailed)
                    {
                        result.ProfanityMessagesCount = totalCount;
                        result.CheckingLog.AddRange(logChecking.Where(x => x.Count != 0));
                    }
                    else if (exectionDetail == EnumExecuteDetail.Verbosity)
                    {
                        result.ProfanityMessagesCount = totalCount;
                        result.CheckingLog.AddRange(logChecking);
                    }
                    else
                        throw new NotSupportedException("Unknown required information");

                    if (result.ProfanityMessagesCount != 0)
                        result.ResultStatus = EnumResultStatus.TextHasProfanity;
                    else
                        result.ResultStatus = EnumResultStatus.TextIsOk;
                }

                if (result.ResultStatus == EnumResultStatus.TextIsOk)
                    result.Decsription = "Text is clear";
                else if (result.ResultStatus == EnumResultStatus.TextHasProfanity)
                    result.Decsription = "Text has profanity";
                else
                    throw new NotSupportedException("Somthing worng");

                result.ExecutionTime = executeSw.Elapsed;
            }
            catch (Exception e)
            {
                result.Decsription = e.Message;
                result.ExecutionTime = executeSw.Elapsed;
                result.ResultStatus = EnumResultStatus.ExecuteError;
            }

            return result;
        }

        /// <summary> Very fast checking </summary>
        /// <param name="inputStream">Input data</param>
        /// <param name="profanities">List of "words"</param>
        /// <remarks>
        /// Currently executing "standart method"...  
        /// </remarks>
        private bool FastCheckInformation(Stream inputStream, IReadOnlyCollection<string> profanities)
        {
            var result = this.CheckProfanity(inputStream, profanities, EnumExecuteDetail.AnyResult);
            var hasAnyFoundedWord = result.Any(x => x.Count != 0);

            return hasAnyFoundedWord;
        }

        /// <summary> Checking text </summary>
        /// <param name="inputStream">Input data</param>
        /// <param name="profanities">List of "words"</param>
        /// <param name="exectionDetail">Requested detail information</param>
        private List<CheckLogInfo> CheckProfanity(Stream inputStream, 
                    IReadOnlyCollection<string> profanities, 
                    EnumExecuteDetail exectionDetail)
        {
            inputStream.Position = 0;
            var textReader = new StreamReader(inputStream, Encoding.UTF8);
            var fullText = textReader.ReadToEnd();
            var checkInfos = new List<CheckLogInfo>();

            foreach (var profanity in profanities)
            {
                var sw = Stopwatch.StartNew();
                var reProfanity = new Regex(@"\b" + profanity + @"\b");
                var profanityCount = reProfanity.Matches(fullText).Count;
                checkInfos.Add(new CheckLogInfo { Profanity = profanity, Count = profanityCount, CheckTime = sw.Elapsed });
            }

            return checkInfos;
        }

        /// <summary> Take Profanities </summary>
        private IReadOnlyCollection<string> GetProfanitiesList()
        {
            return this.ProfanityListService.GetProfanityWordList();
        }
    }
}
