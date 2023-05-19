namespace Server.Contracts.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public bool EmailConfirmed { get; set; }
}