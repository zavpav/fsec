using System.Collections.Generic;
using System.Threading.Tasks;
using ProfanityList.WordList;

namespace CheckProfanityKrestel.Lib
{
    public class ProfanityListInMemoryService : ProfanityListServiceBase
    {
        private List<string> _wordList;

        public ProfanityListInMemoryService()
        {
            this._wordList = new List<string>();
        }

        protected override Task<IReadOnlyCollection<string>> InternalCollection()
        {
            var tcs = new TaskCompletionSource<IReadOnlyCollection<string>>();
            tcs.SetResult(this._wordList);
            return tcs.Task;
        }

        protected override Task InternalAdd(string normalizedWord)
        {
            _wordList.Add(normalizedWord);
            return Task.CompletedTask;
        }

        protected override Task InternalRemove(string normalizedWord)
        {
            _wordList.Remove(normalizedWord);
            return Task.CompletedTask;
        }
    }
}