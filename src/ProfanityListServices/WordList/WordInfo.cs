using System;
using System.Threading;

namespace ProfanityList.WordList
{
    public class WordInfo
    {
        public WordInfo(string normalizeWord)
        {
            this.NormalizeWord = normalizeWord;
        }

        /// <summary> Word </summary>
        public string NormalizeWord { get; set; }
        
        /// <summary> Execution Count (can be zero) </summary>
        public int Count { get; set; }
        
        /// <summary> Total checking time </summary>
        public TimeSpan TotalTime { get; set; }

        public TimeSpan AverageTime()
        {
            if (this.Count == 0)
                return new TimeSpan();
            
            return this.TotalTime / this.Count;
        }
    }
}