using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server.Services;

public class MessageBusSubscriber : BackgroundService
{
    private readonly ILogger<MessageBusSubscriber> _logger;
    
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public MessageBusSubscriber(ILogger<MessageBusSubscriber> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("MyExchange", ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, "MyExchange", string.Empty);

        _logger.LogInformation($"{nameof(MessageBusSubscriber)} initialized");
        _connection.ConnectionShutdown += (_, _) =>
            _logger.LogInformation($"{nameof(MessageBusSubscriber)}: connection shutdown");
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            var message = Serializer.Deserialize<string>(ea.Body);
            _logger.LogInformation("Message received: {@Message}", message);
        };

        _channel.BasicConsume(_queueName, true, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }
}