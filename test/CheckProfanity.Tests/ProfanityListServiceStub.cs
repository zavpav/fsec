using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfanityList.WordList;
using Serilog;

namespace CheckProfanity.Tests
{
    public class ProfanityListServiceStub : IProfanityListService
    {
        private List<WordInfo> _list;

        public ProfanityListServiceStub(List<string> profanityWords)
        {
            this._list = profanityWords.Select(x => new WordInfo(x)).ToList();
        }

        public Task<IReadOnlyCollection<WordInfo>> GetProfanityWordList()
        {
            var tcs = new TaskCompletionSource<IReadOnlyCollection<WordInfo>>();
            tcs.SetResult(this._list);
            return tcs.Task;
        }

        public void SetLogger(ILogger? logger)
        {
        }

        public Task<BasketEditResult> Add(string word)
        {
            var tcs = new TaskCompletionSource<BasketEditResult>();
            tcs.SetResult(
                new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Ok,
                    Word = word,
                    Description = "Test"
                }
            );
            return tcs.Task;
        }

        public Task<BasketEditResult> Remove(string word)
        {
            var tcs = new TaskCompletionSource<BasketEditResult>();
            tcs.SetResult(
                new BasketEditResult
                {
                    Result = BasketEditResult.EnumResult.Ok,
                    Word = word,
                    Description = "Test"
                }
            );
            return tcs.Task;
        }

        public Task SaveStat(string word, TimeSpan time)
        {
            return Task.CompletedTask;
        }
    }
}