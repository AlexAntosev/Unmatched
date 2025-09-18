namespace Unmatched.StatisticsService.Domain.Communication.Consumers;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using Unmatched.StatisticsService.Domain.Communication.Match.Kafka;
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

        var allHeroStats = await unitOfWork.HeroStats.GetAllAsync();
        foreach (var fighter in matchCreated.Fighters)
        {
            var heroStats = allHeroStats.FirstOrDefault(x => x.HeroId == fighter.HeroId);
            if (heroStats != null)
            {
                heroStats.LastMatchIncludedAt = matchCreated.Date;
                heroStats.ModifiedAt = DateTime.UtcNow;
                heroStats.LastMatchPoints = fighter.MatchPoints ?? 0;
                heroStats.Points = fighter.ResultRating ?? 0;
                heroStats.TotalMatches++;
                heroStats.Place = 999; // TODO: implement
                if (fighter.IsWinner)
                {
                    heroStats.TotalWins++;
                }
                else
                {
                    heroStats.TotalLooses++;
                }
            }
            else
            {
                throw new KeyNotFoundException($"There is no hero stats entry found for hero: {fighter.HeroId}");
            }
        }

        var updatedStats = heroPlaceAdjuster.Adjust(allHeroStats);
        await unitOfWork.HeroStats.AddOrUpdateAsync(updatedStats);

        await unitOfWork.SaveChangesAsync();
    }
}

public interface IMatchCreatedHandler
{
    Task HandleAsync(string messageKey, MatchCreated messageValue);
}
