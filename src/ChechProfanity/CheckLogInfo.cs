using System;

namespace CheckProfanity
{
    /// <summary> Information during check </summary>
    public class CheckLogInfo
    {
        /// <summary> "Word" </summary>
        public string Profanity { get; set; }

        /// <summary> Count </summary>
        public int Count { get; set; }

        /// <summary> Checking time </summary>
        public TimeSpan CheckTime { get; set; }

    }
}