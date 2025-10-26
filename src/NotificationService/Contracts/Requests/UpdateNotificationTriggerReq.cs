namespace NotificationService.Contracts.Requests;

public class UpdateNotificationTriggerReq
{
    public required string Subject { get; set; }
    
    public required string LiquidTemplate { get; set; }
}