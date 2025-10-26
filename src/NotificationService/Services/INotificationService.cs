using NotificationService.Contracts.Dtos;
using ProtobufSpec.Events;

namespace NotificationService.Services;

public interface INotificationService
{
    Task<NotificationTriggerDto> CreateNotificationTriggerAsync(string triggerName, string subject, string liquidTemplate);
    Task<NotificationTriggerDto?> GetNotificationTriggerAsync(string triggerName, CancellationToken ct = default);
    Task<List<NotificationTriggerDto>> GetNotificationTriggersAsync(CancellationToken ct = default);
    Task<bool> UpdateNotificationTriggerAsync(string triggerName, string subject, string liquidTemplate, CancellationToken ct = default);
    Task<bool> DeleteNotificationTriggerAsync(string triggerName, CancellationToken ct = default);
    Task HandleDomainEventAsync(string triggerName, DomainEvent message, CancellationToken ct = default);
    Task SendNotificationAsync(Guid userId, NotificationTriggerDto notificationTriggerDto, object message,  CancellationToken ct = default);
}