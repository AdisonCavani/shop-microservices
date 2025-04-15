using ProtoBuf;

namespace ProtobufSpec.Events;

[ProtoContract]
public class OrderCompletedEmailEvent
{
    [ProtoMember(1)]
    public required Guid UserId { get; set; }
    
    [ProtoMember(2)]
    public required string ActivationCode { get; set; }
}