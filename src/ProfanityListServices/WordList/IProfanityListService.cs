using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
#nullable enable

namespace ProfanityList.WordList
{
    /// <summary>
    /// Service for administrating a profanity word list
    /// </summary>
    public interface IProfanityListService
    {
        /// <summary> Set logger if needs </summary>
        void SetLogger(ILogger? logger);

        /// <summary> Return all registred words </summary>
        Task<IReadOnlyCollection<WordInfo>> GetProfanityWordList();

        /// <summary> Add word into the basket </summary>
        Task<BasketEditResult> Add(string word);

        /// <summary> Remove word into the basket </summary>
        Task<BasketEditResult> Remove(string word);

        /// <summary> Save statistic </summary>
        /// <param name="word">Word</param>
        /// <param name="time">Checking Time</param>
        /// <returns>void</returns>
        Task SaveStat(string word, TimeSpan time);
    }
}