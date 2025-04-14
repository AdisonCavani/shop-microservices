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
    internal static async Task<Results<StatusCodeHttpResult, UnauthorizedHttpResult, NoContent, Ok>> HandleAsync(
        HttpRequest request,
        [FromServices] IOptions<AppSettings> appSettings,
        [FromServices] Publisher<PaymentSucceededEvent> publisher)
    {
        try
        {
            var payload = await new StreamReader(request.Body).ReadToEndAsync();

            if (!request.Headers.TryGetValue("Stripe-Signature", out var signature))
                return TypedResults.Unauthorized();
        
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, appSettings.Value.Stripe.WebhookSecret);

            if (stripeEvent.Type != "charge.succeeded")
                return TypedResults.NoContent();

            var charge = stripeEvent.Data.Object as Charge;

            if (charge is null || !charge.Paid)
                return TypedResults.NoContent();

            await publisher.PublishEventAsync(new PaymentSucceededEvent
            {
                PaymentId = Guid.Parse(charge.Metadata["PaymentId"])
            });
        
            return TypedResults.Ok();
        }
        catch (StripeException)
        {
            return TypedResults.Unauthorized();
        }
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "Stripe webhook";

        return operation;
    }
}