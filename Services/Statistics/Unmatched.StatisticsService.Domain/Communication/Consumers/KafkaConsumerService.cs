namespace Unmatched.StatisticsService.Domain.Communication.Consumers;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public abstract class KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
{
    protected readonly IServiceScopeFactory ScopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = GetConsumerConfig(configuration);
        var topic = GetTopic(configuration);

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        await KafkaUtils.WaitForTopicAsync(topic, config.BootstrapServers, TimeSpan.FromSeconds(5), stoppingToken, logger);

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        logger.LogInformation("Kafka consumer subscribed to topic {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);

                    logger.LogInformation("Consumed message {Key}:{Value} at {TopicPartitionOffset}", cr.Message.Key, cr.Message.Value, cr.TopicPartitionOffset);

                    await HandleEventAsync(cr, stoppingToken);

                    consumer.Commit(cr); // commit only after successful handling
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                    await Task.Delay(1000, stoppingToken); // simple backoff
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error processing Kafka message");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }
        finally
        {
            consumer.Close(); // gracefully leave the group
            logger.LogInformation("Kafka consumer closed");
        }
    }

    protected abstract ConsumerConfig GetConsumerConfig(IConfiguration configuration);

    protected abstract string GetTopic(IConfiguration configuration);

    protected abstract Task HandleEventAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken);
}