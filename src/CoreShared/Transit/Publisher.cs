using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;

namespace CoreShared.Transit;

public class Publisher<TEventType> : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;

    public Publisher(ILogger<Publisher<TEventType>> logger, IConnection connection)
    {
        _exchangeName = typeof(TEventType).Name + "Exchange";
        _connection = connection;
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);

        logger.LogInformation($"{nameof(Publisher<TEventType>)} initialized");
        
        _connection.ConnectionShutdown += (_, _) =>
            logger.LogInformation($"{nameof(Publisher<TEventType>)}: connection shutdown");
    }
    
    public void PublishEvent(TEventType eventModel)
    {
        if (!_connection.IsOpen)
            return;

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, eventModel);
        var body = stream.ToArray();

        _channel.BasicPublish(_exchangeName, string.Empty, null, body);
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