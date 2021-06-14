using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public static class ProfanityListFactory
    {
        public static IProfanityListService GetProfanityListService()
        {
            return new ProfanityListDynamoService();
        }
    }
}