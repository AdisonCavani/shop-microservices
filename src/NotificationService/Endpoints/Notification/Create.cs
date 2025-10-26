using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotificationService.Contracts.Dtos;
using NotificationService.Contracts.Requests;
using NotificationService.Services;
using ProtobufSpec;

namespace NotificationService.Endpoints.Notification;

public static class Create
{
    internal static async Task<Results<StatusCodeHttpResult, Created<NotificationTriggerDto>>> HandleAsync(
        [FromBody] CreateNotificationTriggerReq req,
        [FromServices] INotificationService notificationService)
    {
        var notificationTriggerDto = await notificationService.CreateNotificationTriggerAsync(req.TriggerName, req.Subject, req.LiquidTemplate);
        return TypedResults.Created($"{ApiRoutes.Notification.ById}/{notificationTriggerDto.TriggerName}", notificationTriggerDto);
    }
    
    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Create notification trigger";

        return operation;
    }
}