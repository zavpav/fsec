using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class ProfanityListDynamoService : ProfanityListServiceBase
    {
        public ProfanityListDynamoService()
        {
        }

        protected override async Task<IReadOnlyCollection<string>> InternalCollection()
        {
            var list = new List<string>();
            using var dyCli = new AmazonDynamoDBClient();

            var scan = await dyCli.ScanAsync(new ScanRequest("WordsTable"));
            foreach (var wrdItm in scan.Items)
                list.Add(wrdItm["Word"].S);

            return list;
        }

        private async Task SaveCollection(List<string> words)
        {
            using var dyCli = new AmazonDynamoDBClient();
            foreach (var word in words)
            {
                await dyCli.PutItemAsync(new PutItemRequest("WordsTable",
                    new Dictionary<string, AttributeValue>(
                            new Dictionary<string, AttributeValue>
                            {
                                {"Word", new AttributeValue {S = word}}
                            }
                        )
                    )
                );
            }
        }

        protected override async Task InternalAdd(string normalizedWord)
        {
            // Code has atomicity problem but I don't want check it now.

            var words = (await this.InternalCollection()).ToList();
            words.Add(normalizedWord);
            await this.SaveCollection(words);
        }

        protected override async Task InternalRemove(string normalizedWord)
        {
            // Code has atomicity problem but I don't want check it now.

            var words = (await this.InternalCollection()).ToList();
            words.Remove(normalizedWord);
            await this.SaveCollection(words);
        }
    }
}