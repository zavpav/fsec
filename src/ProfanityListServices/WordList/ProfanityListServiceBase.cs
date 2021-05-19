using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfanityList.WordList
{
    public abstract class ProfanityListServiceBase : IProfanityListService
    {
        /// <summary> Internal list of words </summary>
        protected abstract Task<IReadOnlyCollection<string>> InternalCollection();

        /// <summary> Internal word adding </summary>
        protected abstract Task InternalAdd(string normalizedWord);

        /// <summary> Internal word removing </summary>
        protected abstract Task InternalRemove(string normalizedWord);

        /// <summary> Normalized word already added </summary>
        private async Task<bool> IsExists(string normalizeWord)
        {
            var words = await this.InternalCollection();
            return words.Contains(normalizeWord);
        }

        /// <summary> Normalize word </summary>
        private string NormalizeWord(string word)
        {
            return word.ToLower();
        }

        /// <summary> List of words </summary>
        public Task<IReadOnlyCollection<string>> GetProfanityWordList()
        {
            return InternalCollection();
        }

        /// <summary> Adding word </summary>
        public async Task<BasketEditResult> Add(string word)
        {
            var normWord = this.NormalizeWord(word);
            var isExists = await this.IsExists(word);

            if (isExists)
            {
                return new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Error,
                    Word = normWord,
                    Description = "Word exists"
                };
            }

            try
            {
                await this.InternalAdd(normWord);
            }
            catch (Exception e)
            {
                return new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Error,
                    Word = normWord,
                    Description = "Internal add error " + e.Message
                };
            }

            return new BasketEditResult
            {
                Result = BasketEditResult.EnumResult.Ok,
                Word = normWord,
                Description = "Word added"
            };
        }

        public async Task<BasketEditResult> Remove(string word)
        {
            var normWord = this.NormalizeWord(word);
            var isExists = await this.IsExists(word);

            if (!isExists)
            {
                return new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Error,
                    Word = normWord,
                    Description = "Word doesn't exists"
                };
            }

            try
            {
                await this.InternalRemove(normWord);
            }
            catch (Exception e)
            {
                return new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Error,
                    Word = normWord,
                    Description = "Internal remove error " + e.Message
                };
            }

            return new BasketEditResult
            {
                Result = BasketEditResult.EnumResult.Ok,
                Word = normWord,
                Description = "Word removed"
            };
        }

    }
}