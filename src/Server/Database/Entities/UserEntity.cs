namespace Server.Database.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public required string Email { get; set; }

    public string Password { get; set; } = default!;
}