using System;
using Amazon;
using Amazon.CloudWatchLogs;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.AwsCloudWatch;

namespace CheckProfanityAwsLambda
{
    public static class LambdaLoggerExtension
    {
        /// <summary> Create logger </summary>
        public static ILogger? TryCreateSerilogLogger()
        {
            try
            {
                var logStreamProvider = new DefaultLogStreamProvider();

                var serilogSinkConfig = new CloudWatchSinkOptions
                {
                    LogGroupName = "Serilog",
                    MinimumLogEventLevel = LogEventLevel.Information,
                    CreateLogGroup = false,
                    TextFormatter = new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"),
                    LogStreamNameProvider = logStreamProvider,
                    RetryAttempts = 5
                };
                var amazonCloudWatchLogger = new AmazonCloudWatchLogsClient();
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.AmazonCloudWatch(serilogSinkConfig, amazonCloudWatchLogger)
                    .CreateLogger();

                logger.Information("Start");

                return logger;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> Dispose logger if it is not null </summary>
        public static void TryDispose(this ILogger? logger)
        {
            logger?.Information("Finish");
            var dsp = logger as IDisposable;
            dsp?.Dispose();
        }
    }
}