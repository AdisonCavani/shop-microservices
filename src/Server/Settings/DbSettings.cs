namespace Server.Settings;

public class DbSettings
{
    public required string PostgresConnectionString { get; set; }
    
    public required string RedisConnectionString { get; set; }
}