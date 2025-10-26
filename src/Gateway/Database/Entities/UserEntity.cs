using ProtobufSpec;

namespace Gateway.Database.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public bool EmailConfirmed { get; set; }

    public required string Password { get; set; }

    public UserRoleEnum UserRole { get; set; } = UserRoleEnum.User;
}