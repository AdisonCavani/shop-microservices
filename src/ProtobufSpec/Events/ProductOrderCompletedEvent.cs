namespace ProtobufSpec.Events;

public class ProductOrderCompletedEvent : DomainEvent
{
    public required string ActivationCode { get; set; }
}