namespace ProtobufSpec.Events;

public abstract class DomainEvent
{
    public Guid? UserId { get; init; }
}