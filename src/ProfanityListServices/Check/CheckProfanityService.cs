using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProfanityList.WordList;
using Serilog;

namespace ProfanityList.Check
{
    public class CheckProfanityService
    {
        private long MAX_FILE_LEN = 10000;

        private IProfanityListService ProfanityListService { get; set; }

        private ILogger? _logger;

        public CheckProfanityService(IProfanityListService profanityListService, ILogger? logger)
        {
            this._logger = logger;
            this.ProfanityListService = profanityListService;
            this.ProfanityListService.SetLogger(logger);
        }

        /// <summary> Profanity words checker </summary>
        /// <param name="inputStream">Income text in UTF-8 encoding</param>
        /// <param name="exectionDetail">Required execution detalization</param>
        /// <returns>Information about checking the inputStream text</returns>
        public async Task<CheckProfanityResult> CheckProfanity(Stream inputStream, EnumExecuteDetail exectionDetail)
        {
            var executeSw = Stopwatch.StartNew();
            
            if (inputStream.Length > this.MAX_FILE_LEN)
            {
                this._logger?.Error("File too long");

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
                this._logger?.Information("CheckProfanity started with " + exectionDetail);
                var profanities = await this.GetProfanitiesList();
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
                    throw new NotSupportedException("Something worng");

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
        private bool FastCheckInformation(Stream inputStream, IReadOnlyCollection<WordInfo> profanities)
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
                    IReadOnlyCollection<WordInfo> profanities, 
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
                this._logger?.Information("Check word: {Word}. Time: {Time}. Founded Count: {IsFoundCount}", 
                    profanity, sw.Elapsed, profanityCount);
                checkInfos.Add(new CheckLogInfo { Profanity = profanity.NormalizeWord, Count = profanityCount, CheckTime = sw.Elapsed });
            }

            return checkInfos;
        }

        /// <summary> Take Profanities </summary>
        private async Task<IReadOnlyCollection<WordInfo>> GetProfanitiesList()
        {
            return await this.ProfanityListService.GetProfanityWordList();
        }
    }
}
