using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotificationService.Services;

namespace NotificationService.Endpoints.Notification;

public static class Delete
{
    internal static async Task<Results<NotFound, Ok>> HandleAsync(
        [FromRoute] string triggerName,
        [FromServices] INotificationService notificationService)
    {
        var removed = await notificationService.DeleteNotificationTriggerAsync(triggerName);
        
        if (removed)
            return TypedResults.Ok();
        
        return TypedResults.NotFound();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Delete notification trigger by name";

        return operation;
    }
}