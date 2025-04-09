using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;

namespace CoreShared.Transit;

public class Publisher<TEventType>(IConnection connection, ILogger<Publisher<TEventType>> logger) : IAsyncDisposable
{
    private IChannel? _channel;
    private string? _exchangeName;

    private async Task InitializeAsync()
    {
        _channel = await connection.CreateChannelAsync();

        _exchangeName = typeof(TEventType).Name + "Exchange";
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout);

        logger.LogInformation($"{nameof(Publisher<TEventType>)} initialized");

        connection.ConnectionShutdownAsync += (_, _) =>
        {
            logger.LogInformation($"{nameof(Publisher<TEventType>)}: connection shutdown");
            return Task.CompletedTask;
        };
    }
    
    public async Task PublishEvent(TEventType eventModel)
    {
        if (_channel is null || !_channel.IsOpen)
            await InitializeAsync();

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, eventModel);
        var body = stream.ToArray();

        await _channel!.BasicPublishAsync(_exchangeName!, string.Empty, body);
    }

    public async ValueTask DisposeAsync()
    {
        if (connection.IsOpen)
            await connection.DisposeAsync();
        
        if (_channel is not null && _channel.IsOpen)
            await _channel.DisposeAsync();
    }
}