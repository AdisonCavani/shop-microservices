namespace ProtobufSpec.Events;

public class PaymentSucceededEvent : DomainEvent
{
    public required Guid PaymentId { get; set; }
}