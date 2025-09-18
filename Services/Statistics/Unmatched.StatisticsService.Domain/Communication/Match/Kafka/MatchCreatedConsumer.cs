using Unmatched.StatisticsService.Domain.Communication.Consumers;

namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedConsumer(ILogger<KafkaConsumerService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IHeroPlaceAdjuster heroPlaceAdjuster) : KafkaConsumerService(
    logger,
    scopeFactory,
    configuration)
{
    protected override ConsumerConfig GetConsumerConfig(IConfiguration configuration)
    {
        return new()
            {
                BootstrapServers = configuration["Services:Kafka:Url"],
                GroupId = "statistics-service",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // safer manual commits
                AllowAutoCreateTopics = false,
                EnablePartitionEof = false
            };
    }

    protected override string GetTopic(IConfiguration configuration)
    {
        return "match-created"; // TODO: move to config or common place
    }

    protected override async Task HandleEventAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        using var scope = ScopeFactory.CreateScope();
        var unitOfWork= scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var matchCreated = JsonSerializer.Deserialize<MatchCreated>(consumeResult.Message.Value);

        var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMatchCreatedHandler>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(unitOfWork, matchCreated);
        }

        await unitOfWork.SaveChangesAsync();
    }
}