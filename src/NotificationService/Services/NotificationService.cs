using CoreShared;
using Fluid;
using Microsoft.EntityFrameworkCore;
using NotificationService.Contracts.Dtos;
using NotificationService.Database;
using NotificationService.Database.Entities;
using NotificationService.Mappers;
using Gateway;
using ProtobufSpec.Events;

namespace NotificationService.Services;

public class NotificationService(
    AppDbContext dbContext,
    FluidParser fluidParser,
    IdentityAPI.IdentityAPIClient identityClient,
    EmailService emailService) : INotificationService
{
    public async Task<NotificationTriggerDto> CreateNotificationTriggerAsync(string triggerName, string subject, string liquidTemplate)
    {
        var available = typeof(DomainEvent).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(DomainEvent).IsAssignableFrom(t))
            .ToList();
        
        var supported = available.Any(t => t.Name.Equals(triggerName));

        if (!supported)
            throw new ProblemException(ExceptionMessages.NotificationTriggerMissing, string.Join(", ", available.Select(t => t.Name).ToList()));
        
        var notificationTriggerExists = await dbContext.NotificationTriggers
            .AsNoTracking()
            .AnyAsync(x => x.TriggerName == triggerName);
        
        if (notificationTriggerExists)
            throw new ProblemException(ExceptionMessages.NotificationTriggerExists, "Notification trigger already exists");
        
        if (!fluidParser.TryParse(liquidTemplate, out _, out var errors))
            throw new ProblemException(ExceptionMessages.InvalidLiquidTemplate, string.Join(Environment.NewLine, errors));

        var notificationTriggerEntity = new NotificationTriggerEntity
        {
            TriggerName = triggerName,
            Subject = subject,
            LiquidTemplate = liquidTemplate
        };
        
        dbContext.NotificationTriggers.Add(notificationTriggerEntity);
        await dbContext.SaveChangesAsync();

        return notificationTriggerEntity.ToNotificationTriggerDto();
    }

    public async Task<NotificationTriggerDto?> GetNotificationTriggerAsync(string triggerName, CancellationToken ct = default)
    {
        var entity = await dbContext.NotificationTriggers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TriggerName == triggerName, ct);
        
        return entity?.ToNotificationTriggerDto();
    }

    public async Task<List<NotificationTriggerDto>> GetNotificationTriggersAsync(CancellationToken ct = default)
    {
        var entities = await dbContext.NotificationTriggers.AsNoTracking().ToListAsync(ct);
        return entities.Select(x => x.ToNotificationTriggerDto()).ToList();
    }

    public async Task<bool> UpdateNotificationTriggerAsync(string triggerName, string subject, string liquidTemplate, CancellationToken ct = default)
    {
        var entity = await dbContext.NotificationTriggers
                .FirstOrDefaultAsync(x => x.TriggerName == triggerName, ct);

        if (entity is null)
            return false;
        
        entity.Subject = subject;
        entity.LiquidTemplate = liquidTemplate;

        return await dbContext.SaveChangesAsync(ct) != 0;
    }

    public async Task<bool> DeleteNotificationTriggerAsync(string triggerName, CancellationToken ct = default)
    {
        return await dbContext.NotificationTriggers
            .Where(x => x.TriggerName == triggerName)
            .ExecuteDeleteAsync(ct) != 0;
    }

    public async Task HandleDomainEventAsync(string triggerName, DomainEvent message, CancellationToken ct = default)
    {
        var notificationTriggerDto = await GetNotificationTriggerAsync(triggerName, ct);

        if (notificationTriggerDto is null)
            return;

        if (message.UserId is null)
            throw new Exception(ExceptionMessages.MissingUserForNotification);

        await SendNotificationAsync(message.UserId.Value, notificationTriggerDto, message, ct);
    }

    public async Task SendNotificationAsync(
        Guid userId,
        NotificationTriggerDto notificationTriggerDto,
        object message,
        CancellationToken ct = default)
    {
        var user = await identityClient.GetUserAsync(new GetUserReq
        {
            Id = userId.ToString()
        });

        if (user is null)
            throw new Exception(ExceptionMessages.UserLost);

        if (!fluidParser.TryParse(notificationTriggerDto.LiquidTemplate, out var template, out var errors))
            throw new Exception(string.Join(Environment.NewLine, errors));

        var templateVariables = new Dictionary<string, object>
        {
            ["FirstName"] = user.FirstName,
            ["LastName"] = user.LastName
        };

        foreach (var prop in message.GetType().GetProperties())
        {
            templateVariables[prop.Name] = prop.GetValue(message) ?? "";
        }

        // TODO: fail when missing variables
        var context = new TemplateContext(templateVariables);
        // Options = {
        //     MemberAccessStrategy = new UnsafeMemberAccessStrategy();
        // };

        // TODO: upgrade fluid package
        var bodyHtml = await template.RenderAsync(context);

        if (bodyHtml is null)
            throw new Exception(ExceptionMessages.ProblemWithRenderingLiquidTemplate);
        
        await emailService.SendEmailAsync(
            $"{user.FirstName} {user.LastName}",
            user.Email,
            notificationTriggerDto.Subject,
            bodyHtml,
            ct);
    }
}