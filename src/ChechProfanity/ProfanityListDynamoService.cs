using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class ProfanityListDynamoService : ProfanityListServiceBase
    {
        public ProfanityListDynamoService()
        {
        }

        protected override async Task<IReadOnlyCollection<WordInfo>> InternalCollection()
        {
            var list = new List<WordInfo>();
            using var dyCli = new AmazonDynamoDBClient();

            var scan = await dyCli.ScanAsync(new ScanRequest("WordsTable"));
            
            foreach (var wrdItm in scan.Items)
                list.Add(new WordInfo(wrdItm["Word"].S) 
                        { 
                            Count = int.Parse(wrdItm["Cnt"].N),
                            TotalTime = TimeSpan.Parse(wrdItm["Time"].S)
                        });

            return list;
        }

        protected override async Task InternalAdd(string normalizedWord)
        {
            using var dyCli = new AmazonDynamoDBClient();

            await dyCli.PutItemAsync(new PutItemRequest("WordsTable",
                    new Dictionary<string, AttributeValue>(
                            new Dictionary<string, AttributeValue>
                            {
                                {"Word", new AttributeValue {S = normalizedWord}},
                                {"Time", new AttributeValue {S = (new TimeSpan()).ToString()}},
                                {"Cnt", new AttributeValue {N = "0"}}
                            }
                    )
                )
            );
        }

        protected override async Task InternalRemove(string normalizedWord)
        {
            using var dyCli = new AmazonDynamoDBClient();

            await dyCli.DeleteItemAsync(new DeleteItemRequest("WordsTable",
                new Dictionary<string, AttributeValue>(
                        new Dictionary<string, AttributeValue>
                        {
                            {"Word", new AttributeValue {S = normalizedWord}}
                        }
                    )
                )
            );
        }

        public override async Task SaveStat(string word, TimeSpan time)
        {
            this._logger?.Information("Try queue");

            using var sqsCli = new AmazonSQSClient();
            var queueUrl = await sqsCli.GetQueueUrlAsync(new GetQueueUrlRequest("profanity-stat-queue"));

            var aa = await sqsCli.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                MessageBody = "Try  " + word
            });
            this._logger?.Information(aa.MessageId);
        }
    }
}