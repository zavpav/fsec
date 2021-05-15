using System.Collections.Generic;
using ProfanityList;

namespace CheckProfanity.Tests
{
    public class ProfanityListServiceStub : IProfanityListService
    {
        private List<string> _list;

        public ProfanityListServiceStub(List<string> profanityWords)
        {
            this._list = profanityWords;
        }

        public IReadOnlyCollection<string> GetProfanityWordList()
        {
            return this._list;
        }
    }
}