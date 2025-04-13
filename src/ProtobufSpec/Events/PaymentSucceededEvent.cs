using ProtoBuf;

namespace ProtobufSpec.Events;

[ProtoContract]
public class PaymentSucceededEvent
{
    [ProtoMember(1)]
    public required Guid PaymentId { get; set; }
}