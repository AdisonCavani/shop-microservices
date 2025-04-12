namespace ProtobufSpec.Events;

public class PaymentSucceededEvent
{
    public required Guid PaymentId { get; set; }
}