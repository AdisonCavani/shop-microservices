using NotificationService.Contracts.Dtos;

namespace NotificationService.Contracts.Responses;

public class GetNotificationTriggersRes
{
    public required List<NotificationTriggerDto> NotificationTriggers { get; set; } = [];
}