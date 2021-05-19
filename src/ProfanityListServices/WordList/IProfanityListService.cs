using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProfanityList.WordList
{
    /// <summary>
    /// Service for administrating a profanity word list
    /// </summary>
    public interface IProfanityListService
    {
        /// <summary> Return all registred words </summary>
        Task<IReadOnlyCollection<string>> GetProfanityWordList();

        /// <summary> Add word into the basket </summary>
        Task<BasketEditResult> Add(string word);

        /// <summary> Remove word into the basket </summary>
        Task<BasketEditResult> Remove(string word);
    }
}