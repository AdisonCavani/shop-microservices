using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;

namespace Server.Auth;

public class RedisTicketStore : ITicketStore
{
    private const string KeyPrefix = "AuthSessionStore-";
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisTicketStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = KeyPrefix + Guid.NewGuid();
        await RenewAsync(key, ticket);
        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var db = _connectionMultiplexer.GetDatabase();

        var expiresUtc = ticket.Properties.ExpiresUtc;
        var value = TicketSerializer.Default.Serialize(ticket);

        if (expiresUtc.HasValue)
            db.StringSet(key, value, expiresUtc.Value.UtcDateTime.Subtract(DateTime.UtcNow));

        else
            db.StringSet(key, value);

        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var bytes = db.StringGet(key);

        return bytes.IsNull
            ? Task.FromResult<AuthenticationTicket?>(null)
            : Task.FromResult(TicketSerializer.Default.Deserialize(bytes!));
    }

    public async Task RemoveAsync(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}