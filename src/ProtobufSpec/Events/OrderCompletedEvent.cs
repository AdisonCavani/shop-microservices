namespace ProtobufSpec.Events;

public class OrderCompletedEvent : DomainEvent
{
    public required Guid OrderId { get; set; }
    
    public required Guid ProductId { get; set; }
}