using System;
using System.Collections.Generic;

namespace CheckProfanity
{
    /// <summary> Result of analyze </summary>
    public class CheckProfanityResult
    {
        public CheckProfanityResult()
        {
            this.ResultStatus = EnumResultStatus.Undef;
            this.ProfanityMessagesCount = -1;
            this.CheckingLog = new List<CheckLogInfo>();
        }

        /// <summary> Status </summary>
        public EnumResultStatus ResultStatus { get; set; }

        /// <summary> Information about exeucting </summary>
        public string Decsription { get; set; }

        /// <summary> Required income detalization </summary>
        public EnumExecuteDetail VerbosityInfo { get; set; }

        /// <summary> Time of analyze </summary>
        public TimeSpan ExecutionTime { get; set; }

        /// <summary> Count of "Profanity words" </summary>
        public int ProfanityMessagesCount { get; set; }

        /// <summary> "Log" messages </summary>
        public List<CheckLogInfo> CheckingLog { get; set; }
    }
}