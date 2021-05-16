using System.Collections.Generic;
using ProfanityList;
using ProfanityList.WordList;

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

        public BasketEditResult Add(string word)
        {
            return new BasketEditResult
            {
                Result = BasketEditResult.EnumResult.Ok,
                Word = word,
                Description = "Test"
            };
        }

        public BasketEditResult Remove(string word)
        {
            return new BasketEditResult
            {
                Result = BasketEditResult.EnumResult.Ok,
                Word = word,
                Description = "Test"
            };
        }
    }
}