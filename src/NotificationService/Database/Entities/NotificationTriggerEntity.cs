namespace NotificationService.Database.Entities;

public class NotificationTriggerEntity
{
    public required string TriggerName { get; set; }
    
    public required string Subject { get; set; }
    
    public required string LiquidTemplate { get; set; }
}