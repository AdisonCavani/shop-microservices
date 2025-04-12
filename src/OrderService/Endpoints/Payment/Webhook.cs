using System.Diagnostics.CodeAnalysis;
using CoreShared.Transit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OrderService.Startup;
using ProtobufSpec.Events;
using Stripe;

namespace OrderService.Endpoints.Payment;

public static class Webhook
{
    internal static async Task<Results<StatusCodeHttpResult, NoContent>> HandleAsync(
        HttpRequest request,
        [FromServices] IOptions<AppSettings> appSettings,
        Publisher<PaymentSucceededEvent> publisher)
    {
        var payload = await new StreamReader(request.Body).ReadToEndAsync();
        var signature = request.Headers["Stripe-Signature"];
        
        var stripeEvent = EventUtility.ConstructEvent(payload, signature, appSettings.Value.Stripe.WebhookSecret);

        if (stripeEvent.Type != "charge.succeeded")
            return TypedResults.NoContent();

        var charge = stripeEvent.Data.Object as Charge;

        if (charge is null || !charge.Paid)
            return TypedResults.NoContent();

        await publisher.PublishEventAsync(new PaymentSucceededEvent
        {
            PaymentId = Guid.Parse(charge.Metadata["PaymentId"]),
        });
        
        return TypedResults.NoContent();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Stripe webhook";

        return operation;
    }
}