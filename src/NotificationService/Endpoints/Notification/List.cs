using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotificationService.Contracts.Responses;
using NotificationService.Services;

namespace NotificationService.Endpoints.Notification;

public static class List
{
    internal static async Task<Results<NotFound, Ok<GetNotificationTriggersRes>>> HandleAsync(
        [FromServices] INotificationService notificationService)
    {
        return TypedResults.Ok(new GetNotificationTriggersRes
        {
            NotificationTriggers = await notificationService.GetNotificationTriggersAsync()
        });
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "List all notification triggers";

        return operation;
    }
}