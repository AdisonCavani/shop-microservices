namespace ProtobufSpec.Events;

public class UserVerificationCreatedEvent : DomainEvent
{
    public required string Token { get; set; }
}