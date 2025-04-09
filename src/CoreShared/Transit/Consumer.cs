using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CoreShared.Transit;

public abstract class Consumer<TEventType>(IConnection connection, IServiceProvider serviceProvider, ILogger<Consumer<TEventType>> logger) : BackgroundService
{
    private IChannel _channel = null!;
    private string _queueName = null!;

    public override async Task StartAsync(CancellationToken ct)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: ct);
        
        var exchangeName = typeof(TEventType).Name + "Exchange";
        await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, cancellationToken: ct);

        var queue = await _channel.QueueDeclareAsync(cancellationToken: ct);
        _queueName = queue.QueueName;
        await _channel.QueueBindAsync(_queueName, exchangeName, string.Empty, cancellationToken: ct);

        logger.LogInformation($"{nameof(Consumer<TEventType>)} initialized");
        
        connection.ConnectionShutdownAsync += (_, _) =>
        {
            logger.LogInformation($"{nameof(Consumer<TEventType>)}: connection shutdown");
            return Task.CompletedTask;
        };

        await base.StartAsync(ct);
    }

    protected abstract Task Consume(TEventType message, IServiceProvider serviceProvider, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = Serializer.Deserialize<TEventType>(ea.Body);
            await Consume(message, serviceProvider, ct);
        };

        await _channel.BasicConsumeAsync(_queueName, true, consumer, ct);
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (_channel.IsOpen)
        {
            await _channel.CloseAsync(ct);
            await connection.CloseAsync(ct);
        }

        await base.StopAsync(ct);
    }
}