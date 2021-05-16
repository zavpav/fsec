using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfanityList.WordList
{
    public abstract class ProfanityListServiceBase : IProfanityListService
    {
        /// <summary> Internal list of words </summary>
        protected abstract IReadOnlyCollection<string> InternalCollection();

        /// <summary> Internal word adding </summary>
        protected abstract void InternalAdd(string normalizedWord);

        /// <summary> Internal word removing </summary>
        protected abstract void InternalRemove(string normalizedWord);

        /// <summary> Normalized word already added </summary>
        private bool IsExists(string normalizeWord)
        {
            return this.InternalCollection().Contains(normalizeWord);
        }

        /// <summary> Normalize word </summary>
        private string NormalizeWord(string word)
        {
            return word.ToLower();
        }

        /// <summary> List of words </summary>
        public IReadOnlyCollection<string> GetProfanityWordList()
        {
            return InternalCollection();
        }

        /// <summary> Adding word </summary>
        public BasketEditResult Add(string word)
        {
            var normWord = this.NormalizeWord(word);
            var isExists = this.IsExists(word);

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
                this.InternalAdd(normWord);
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


        public BasketEditResult Remove(string word)
        {
            var normWord = this.NormalizeWord(word);
            var isExists = this.IsExists(word);

            if (isExists)
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
                this.InternalRemove(normWord);
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