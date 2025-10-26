using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotificationService.Contracts.Requests;
using NotificationService.Services;

namespace NotificationService.Endpoints.Notification;

public static class Update
{
    internal static async Task<Results<NotFound, Ok>> HandleAsync(
        [FromRoute] string triggerName,
        [FromBody] UpdateNotificationTriggerReq req,
        [FromServices] INotificationService notificationService)
    {
        var updated = await notificationService.UpdateNotificationTriggerAsync(triggerName, req.Subject, req.LiquidTemplate);
        
        if (updated)
            return TypedResults.Ok();

        return TypedResults.NotFound();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Update notification trigger by name";

        return operation;
    }
}