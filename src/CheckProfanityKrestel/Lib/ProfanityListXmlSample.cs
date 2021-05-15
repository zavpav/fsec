using System.Collections.Generic;
using ProfanityList.WordList;

namespace CheckProfanityKrestel.Lib
{
    public class ProfanityListXmlSample : IProfanityListService
    {
        public IReadOnlyCollection<string> GetProfanityWordList()
        {
            return new List<string> { "ah", "oh", "eh" };
        }
    }
}