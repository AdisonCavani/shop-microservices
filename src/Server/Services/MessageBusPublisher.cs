using ProtoBuf;
using RabbitMQ.Client;

namespace Server.Services;

public class MessageBusPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusPublisher(ILogger<MessageBusPublisher> logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
        _connection.ConnectionShutdown += (_, _) =>
            logger.LogInformation($"{nameof(MessageBusPublisher)}: connection shutdown");

        logger.LogInformation($"{nameof(MessageBusPublisher)} initialized");
    }
    
    public void PublishUrlCreatedEvent(string message)
    {
        if (!_connection.IsOpen)
            return;

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, message);
        var body = stream.ToArray();

        _channel.BasicPublish("MyExchange", string.Empty, null, body);
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