using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CoreShared.Transit;

public abstract class Consumer<TEventType> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    protected Consumer(IServiceProvider serviceProvider, ILogger<Consumer<TEventType>> logger, IConnection connection)
    {
        var exchangeName = typeof(TEventType).Name + "Exchange";
        
        _serviceProvider = serviceProvider;
        _connection = connection;
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, exchangeName, string.Empty);

        logger.LogInformation($"{nameof(Consumer<TEventType>)} initialized");
        
        _connection.ConnectionShutdown += (_, _) =>
            logger.LogInformation($"{nameof(Consumer<TEventType>)}: connection shutdown");
    }
    
    protected abstract Task Consume(TEventType message, IServiceProvider serviceProvider, CancellationToken cancellationToken);

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var message = Serializer.Deserialize<TEventType>(ea.Body);
            await Consume(message, _serviceProvider, cancellationToken);
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