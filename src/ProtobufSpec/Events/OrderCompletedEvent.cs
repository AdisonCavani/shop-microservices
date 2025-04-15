using ProtoBuf;

namespace ProtobufSpec.Events;

[ProtoContract]
public class OrderCompletedEvent
{
    [ProtoMember(1)]
    public required Guid OrderId { get; set; }
    
    [ProtoMember(2)]
    public required Guid ProductId { get; set; }
    
    [ProtoMember(3)]
    public required Guid UserId { get; set; }
}