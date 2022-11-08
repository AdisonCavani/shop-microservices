using ProtoBuf;

namespace Server.Contracts.Events;

[ProtoContract]
public class UserCreatedEvent
{
    [ProtoMember(1)]
    public required string Name { get; set; }
    
    [ProtoMember(2)]
    public required string Email { get; set; }
    
    [ProtoMember(3)]
    public required string Token { get; set; }
}