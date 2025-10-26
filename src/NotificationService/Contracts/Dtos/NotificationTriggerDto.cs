namespace NotificationService.Contracts.Dtos;

public class NotificationTriggerDto
{
    public required string TriggerName { get; set; }
    
    public required string Subject { get; set; }
    
    public required string LiquidTemplate { get; set; }
}