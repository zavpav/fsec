// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

using Amazon.Lambda.Core;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class BasketFunctions
    {
        private ProfanityListS3BucketService Basket { get; }

        public BasketFunctions()
        {
            this.Basket = new ProfanityListS3BucketService();
        }
        
        public BasketEditResult BasketAddWordHandler(string word, ILambdaContext context)
        {
            return this.Basket.Add(word);
        }

        public BasketEditResult BasketRemoveWordHandler(string word, ILambdaContext context)
        {
            return this.Basket.Add(word);
        }
    }
}