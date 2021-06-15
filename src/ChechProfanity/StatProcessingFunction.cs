using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace CheckProfanityAwsLambda
{
    public class StatProcessingFunction
    {
        public string ProcessStatEvent(SQSEvent sqsEvent, ILambdaContext context)
        {

            Console.WriteLine($"Beginning to process {sqsEvent.Records.Count} records...");
            try
            {
                var logger2 = LambdaLoggerExtension.TryCreateSerilogLogger();
                logger2.Information("Parse queue");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            foreach (var record in sqsEvent.Records)
            {
                Console.WriteLine($"Message ID: {record.MessageId}");
                Console.WriteLine($"Event Source: {record.EventSource}");

                var logger = LambdaLoggerExtension.TryCreateSerilogLogger();
                logger?.Information("Parse queue" + record.Body);

                
                Console.WriteLine($"Record Body:");
                Console.WriteLine(record.Body);
            }

            Console.WriteLine("Processing complete.");

            return $"Processed {sqsEvent.Records.Count} records.";
        }
    }
}