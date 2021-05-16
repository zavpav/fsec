using System.Collections.Generic;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class ProfanityListS3BucketService : ProfanityListServiceBase
    {
        protected override IReadOnlyCollection<string> InternalCollection()
        {
            return new List<string> { "ah", "oh", "eh" };
        }

        protected override void InternalAdd(string normalizedWord)
        {
            
        }

        protected override void InternalRemove(string normalizedWord)
        {
            
        }
    }
}
