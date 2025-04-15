namespace NotificationService.Templates;

public class OrderCompleted
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string ActivationCode { get; set; }

    public const string Subject = "Order completed";
}