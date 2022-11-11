using ProtoBuf;

namespace Server.Contracts.Events;

[ProtoContract]
public class UserCreatedEvent
{
    [ProtoMember(1)]
    public required string FirstName { get; set; }
    
    [ProtoMember(2)]
    public required string LastName { get; set; }
    
    [ProtoMember(3)]
    public required string Email { get; set; }
    
    [ProtoMember(4)]
    public required string Token { get; set; }
}