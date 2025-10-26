namespace NotificationService.Contracts.Requests;

public class CreateNotificationTriggerReq
{
    public required string TriggerName { get; set; }
    
    public required string Subject { get; set; }
    
    public required string LiquidTemplate { get; set; }
}