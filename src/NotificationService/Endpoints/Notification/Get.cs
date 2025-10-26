using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotificationService.Contracts.Dtos;
using NotificationService.Services;

namespace NotificationService.Endpoints.Notification;

public static class Get
{
    internal static async Task<Results<NotFound, Ok<NotificationTriggerDto>>> HandleAsync(
        [FromRoute] string triggerName,
        [FromServices] INotificationService notificationService)
    {
        var notificationTriggerDto = await notificationService.GetNotificationTriggerAsync(triggerName);

        if (notificationTriggerDto is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(notificationTriggerDto);
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Get notification trigger by name";

        return operation;
    }
}