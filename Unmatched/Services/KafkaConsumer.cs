namespace Unmatched.Services;

using System.Text.Json;

using Confluent.Kafka;
using Unmatched.Services.Contracts;

public class KafkaConsumer<T> : IKafkaConsumer<T>
{
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(string groupId, string topic)
    {
        var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:9092",
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(topic);
    }

    public void Start(Func<T, Task> handleMessage)
    {
        Task.Run(() =>
            {
                while (true)
                {
                    var cr = _consumer.Consume();
                    var message = JsonSerializer.Deserialize<T>(cr.Message.Value);
                    handleMessage(message);
                }
            });
    }
}