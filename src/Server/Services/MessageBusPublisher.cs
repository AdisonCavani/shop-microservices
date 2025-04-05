using ProtoBuf;
using ProtobufSpec;
using RabbitMQ.Client;

namespace Server.Services;

public class MessageBusPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusPublisher(ILogger<MessageBusPublisher> logger, IConnection connection)
    {
        _connection = connection;
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(Exchanges.UserCreatedExchange, ExchangeType.Fanout);
        _connection.ConnectionShutdown += (_, _) =>
            logger.LogInformation($"{nameof(MessageBusPublisher)}: connection shutdown");

        logger.LogInformation($"{nameof(MessageBusPublisher)} initialized");
    }
    
    public void PublishEvent<T>(T eventModel)
    {
        if (!_connection.IsOpen)
            return;

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, eventModel);
        var body = stream.ToArray();

        _channel.BasicPublish(Exchanges.UserCreatedExchange, string.Empty, null, body);
    }

    public void Dispose()
    {
        if (_connection.IsOpen)
        {
            _connection.Dispose();
            _channel.Dispose();
        }
    }
}