using Server.Services;

namespace Server.Resolvers.Queries;

public class Query
{
    public bool Test([Service] MessageBusPublisher publisher, string message)
    {
        publisher.PublishUrlCreatedEvent(message);
        return true;
    }
}