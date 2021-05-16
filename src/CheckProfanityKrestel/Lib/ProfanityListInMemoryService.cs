using System.Collections.Generic;
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

        protected override IReadOnlyCollection<string> InternalCollection()
        {
            return _wordList;
        }

        protected override void InternalAdd(string normalizedWord)
        {
            _wordList.Add(normalizedWord);
        }

        protected override void InternalRemove(string normalizedWord)
        {
            _wordList.Remove(normalizedWord);
        }
    }
}