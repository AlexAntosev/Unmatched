namespace Unmatched.CatalogService.Domain.Services;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T @event);
}