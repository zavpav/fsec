using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using ProfanityList.WordList;

namespace CheckProfanityAwsLambda
{
    public class ProfanityListS3BucketService : ProfanityListServiceBase
    {

        IAmazonS3 S3Client { get; set; }
        
        public ProfanityListS3BucketService()
        {
            this.S3Client = new AmazonS3Client();
        }

        public ProfanityListS3BucketService(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }

        private List<string> _contentCache = null;

        protected override async Task<IReadOnlyCollection<string>> InternalCollection()
        {
            if (this._contentCache == null)
            {
                try
                {
                    // Parallel execution problem
                    this._contentCache = new List<string>();

                    var s3FileResponse = await this.S3Client.GetObjectAsync("profanity.storage", "words.list.xml");
                    using (var reader = new StreamReader(s3FileResponse.ResponseStream))
                    {
                        var contents = reader.ReadToEnd();
                        var xDoc = XElement.Parse(contents);

                        foreach (var xElement in xDoc.Elements("word"))
                            this._contentCache.Add(xElement.Value);
                    }

                }
                catch (AmazonS3Exception e) when (e.Message.Contains("The specified key does not exist"))
                {
                }
                catch (Exception e)
                {
                    throw new Exception("Load words " + e.ToString(), e);
                }
            }

            return this._contentCache;
        }

        private async Task SaveCollection()
        {
            var step = "create1";
            var xDoc = new XElement("root");
            foreach (var word in _contentCache)
                xDoc.Add(new XElement("word", word));

            step = "fill";
            var s3FileRequest = new PutObjectRequest
            {
                BucketName = "SampleBucket",
                Key = "words.list.xml",
                ContentBody = xDoc.ToString(),
                ContentType = "application/xml"

            };

            step = "request";

            try
            {
                await this.S3Client.PutObjectAsync(s3FileRequest);
            }
            catch (Exception e)
            {
                throw new Exception("Save words " + step + " "+ e.ToString(), e);
            }
        }

        protected override async Task InternalAdd(string normalizedWord)
        {
            // Code has atomicity problem but I don't want check it now.

            await this.InternalCollection();
            this._contentCache.Add(normalizedWord);
            await this.SaveCollection();

        }

        protected override async Task InternalRemove(string normalizedWord)
        {
            // Code has atomicity problem but I don't want check it now.

            await this.InternalCollection();
            this._contentCache.Add(normalizedWord);
            await this.SaveCollection();
        }

    }
}
