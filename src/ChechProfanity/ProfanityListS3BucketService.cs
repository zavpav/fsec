using System.Collections.Generic;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class ProfanityListS3BucketService : IProfanityListService
    {
        public IReadOnlyCollection<string> GetProfanityWordList()
        {
            return new List<string> { "ah", "oh", "eh" };
        }
    }
}
