namespace Server.Contracts.Dtos;

public class HealthCheckDto
{
    public required string Status { get; set; }

    public required string Component { get; set; }

    public string? Description { get; set; }
}