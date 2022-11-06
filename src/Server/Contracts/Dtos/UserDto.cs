namespace Server.Contracts.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public required string Email { get; set; }
}