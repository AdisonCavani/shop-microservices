using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Server.Contracts;
using Server.Contracts.Events;

namespace Server.Services;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public MessageBusSubscriber(IServiceProvider serviceProvider, ILogger<MessageBusSubscriber> logger)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(Exchanges.UserCreatedExchange, ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, Exchanges.UserCreatedExchange, string.Empty);

        logger.LogInformation($"{nameof(MessageBusSubscriber)} initialized");
        _connection.ConnectionShutdown += (_, _) =>
            logger.LogInformation($"{nameof(MessageBusSubscriber)}: connection shutdown");
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var message = Serializer.Deserialize<UserCreatedEvent>(ea.Body);

            using var scope = _serviceProvider.CreateScope();
            var emailHandler = scope.ServiceProvider.GetRequiredService<EmailHandler>();

            await emailHandler.VerificationMailAsync(
                message,
                cancellationToken);
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