using System.Collections.Generic;

namespace ProfanityList.WordList
{
    /// <summary>
    /// Service for administrating a profanity word list
    /// </summary>
    public interface IProfanityListService
    {
        /// <summary> Return all registred words </summary>
        /// <returns></returns>
        IReadOnlyCollection<string> GetProfanityWordList();

        /// <summary> Add word into the basket </summary>
        BasketEditResult Add(string word);

        /// <summary> Remove word into the basket </summary>
        BasketEditResult Remove(string word);

    }
}