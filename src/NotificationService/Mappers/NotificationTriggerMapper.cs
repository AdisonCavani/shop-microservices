using NotificationService.Contracts.Dtos;
using NotificationService.Database.Entities;

namespace NotificationService.Mappers;

public static class NotificationTriggerMapper
{
    public static NotificationTriggerDto ToNotificationTriggerDto(
        this NotificationTriggerEntity notificationTriggerEntity)
    {
        return new NotificationTriggerDto
        {
            TriggerName = notificationTriggerEntity.TriggerName,
            Subject = notificationTriggerEntity.Subject,
            LiquidTemplate = notificationTriggerEntity.LiquidTemplate
        };
    }
}