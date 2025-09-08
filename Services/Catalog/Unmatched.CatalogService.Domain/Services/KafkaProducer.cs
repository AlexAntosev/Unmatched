namespace Unmatched.CatalogService.Domain.Services;

using System.Text.Json;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var bootstrapServers = configuration["Services:Kafka:Url"];
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T @event)
    {
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(@event);

        await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = value
            });
    }
}