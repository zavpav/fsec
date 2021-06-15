using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace ProfanityList.WordList
{
    public abstract class ProfanityListServiceBase : IProfanityListService
    {
        /// <summary> Internal list of words </summary>
        protected abstract Task<IReadOnlyCollection<WordInfo>> InternalCollection();

        /// <summary> Internal word adding </summary>
        protected abstract Task InternalAdd(string normalizedWord);

        /// <summary> Internal word removing </summary>
        protected abstract Task InternalRemove(string normalizedWord);

        public abstract Task SaveStat(string word, TimeSpan time);


        /// <summary> Normalized word already added </summary>
        private async Task<bool> IsExists(string normalizeWord)
        {
            var words = await this.InternalCollection();
            return words.Any(x => x.NormalizeWord == normalizeWord);
        }

        /// <summary> Normalize word </summary>
        private string NormalizeWord(string word)
        {
            return word.ToLower();
        }

        public Task<IReadOnlyCollection<WordInfo>> GetProfanityWordList()
        {
            return InternalCollection();
        }


        protected ILogger? _logger;
        public void SetLogger(ILogger? logger)
        {
            this._logger = logger;
        }

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
                var sw = Stopwatch.StartNew();
                await this.InternalAdd(normWord);
                this._logger?.Information("Add word {Word} Time {Time}", word, sw.Elapsed);
            }
            catch (Exception e)
            {
                this._logger?.Information(e, "Error adding word {Word}", word);

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
                var sw = Stopwatch.StartNew();
                await this.InternalRemove(normWord);
                this._logger?.Information("Remove word {Word} Time {Time}", word, sw.Elapsed);
            }
            catch (Exception e)
            {
                this._logger?.Information(e, "Error removing word {Word}", word);
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