namespace Unmatched.StatisticsService.Domain.Communication.Consumers;

using Confluent.Kafka;

using Microsoft.Extensions.Logging;

public static class KafkaUtils
{
    public static async Task WaitForTopicAsync(
        string topic,
        string bootstrapServers,
        TimeSpan pollInterval,
        CancellationToken cancellationToken,
        ILogger logger)
    {
        using var admin = new AdminClientBuilder(
            new AdminClientConfig
                {
                    BootstrapServers = bootstrapServers
                }).Build();

        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                // Query metadata for the specific topic
                var metadata = admin.GetMetadata(topic, TimeSpan.FromSeconds(5));

                // If partitions > 0, the topic exists
                if (metadata.Topics.Any(t => t.Topic == topic && !t.Error.IsError && t.Partitions.Count > 0))
                {
                    logger.LogInformation($"Topic '{topic}' is available.");
                    return;
                }
            }
            catch (KafkaException ex) 
            {
                // Topic does not exist yet — normal in this scenario
                logger.LogWarning($"Topic '{topic}' not found yet.");
            }

            await Task.Delay(pollInterval, cancellationToken);
        }

        //throw new TimeoutException($"Topic '{topic}' was not created after {maxRetries} attempts.");
    }
}
