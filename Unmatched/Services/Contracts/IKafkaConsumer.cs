namespace Unmatched.Services.Contracts;

public interface IKafkaConsumer<T>
{
    void Start(Func<T, Task> handleMessage);
}