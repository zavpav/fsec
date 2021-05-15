using System;
using System.Collections.Generic;

namespace ProfanityList
{
    public class ProfanityListS3BucketService : IProfanityListService
    {
        public IReadOnlyCollection<string> GetProfanityWordList()
        {
            return new List<string> { "ah", "oh", "eh" };
        }
    }
}
