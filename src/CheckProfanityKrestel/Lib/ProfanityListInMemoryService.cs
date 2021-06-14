using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfanityList.WordList;

namespace CheckProfanityKrestel.Lib
{
    public class ProfanityListInMemoryService : ProfanityListServiceBase
    {
        private List<WordInfo> _wordList;

        public ProfanityListInMemoryService()
        {
            this._wordList = new List<WordInfo>();
        }

        protected override Task<IReadOnlyCollection<WordInfo>> InternalCollection()
        {
            var tcs = new TaskCompletionSource<IReadOnlyCollection<WordInfo>>();
            tcs.SetResult(this._wordList);
            return tcs.Task;
        }

        protected override Task InternalAdd(string normalizedWord)
        {
            _wordList.Add(new WordInfo(normalizedWord));
            return Task.CompletedTask;
        }

        protected override Task InternalRemove(string normalizedWord)
        {
            _wordList.RemoveAll(x => x.NormalizeWord == normalizedWord);
            return Task.CompletedTask;
        }

        public override Task SaveStat(string word, TimeSpan time)
        {
            return Task.CompletedTask;
        }
    }
}