using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Stripe;

namespace OrderService.Endpoints.Payment;

public static class Webhook
{
    internal static async Task<Results<StatusCodeHttpResult, NoContent>> HandleAsync(HttpContext context)
    {
        var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var webhookSecret = "my-webhook-secret";
        var signature = context.Request.Headers["Stripe-Signature"];
        
        var stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);

        if (stripeEvent.Type != "charge.succeeded")
            return TypedResults.NoContent();

        var charge = stripeEvent.Data.Object as Charge;

        if (charge is null || !charge.Paid)
            return TypedResults.NoContent();
        
        
        
        return TypedResults.NoContent();
    }

    [ExcludeFromCodeCoverage]
    internal static OpenApiOperation OpenApi(OpenApiOperation operation)
    {
        operation.Summary = "";

        return operation;
    }
}