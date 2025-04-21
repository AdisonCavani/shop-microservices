extern alias ordersvc;
using ordersvc::OrderService.Database.Entities;

namespace OrderService.UnitTests;

internal static class Helpers
{
    internal static Tuple<List<OrderEntity>, List<PaymentEntity>> GetDbEntities()
    {
        var sharedUser = Guid.NewGuid();
        
        var orderIdWithExpiredPayments = Guid.NewGuid();
        var orderIdWithPendingPayment = Guid.NewGuid();
        var orderIdWithFinishedPayment = Guid.NewGuid();
        
        var payments = new List<PaymentEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                StripeCheckoutId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                ExpiresAt = DateTime.UtcNow.AddDays(-2),
                OrderId = orderIdWithExpiredPayments,
            },
            new()
            {
                Id = Guid.NewGuid(),
                StripeCheckoutId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                OrderId = orderIdWithPendingPayment,
            },
            new()
            {
                Id = Guid.NewGuid(),
                StripeCheckoutId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                OrderId = orderIdWithFinishedPayment
            },
            new()
            {
                Id = Guid.NewGuid(),
                StripeCheckoutId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ExpiresAt = DateTime.UtcNow,
                Paid = true,
                OrderId = orderIdWithFinishedPayment
            }
        };
        
        var orders = new List<OrderEntity>()
        {
            new()
            {
              Id = Guid.NewGuid(),
              UserId = sharedUser,
              ProductId = Guid.NewGuid(),
            },
            new()
            {
                Id = orderIdWithExpiredPayments,
                UserId = sharedUser,
                ProductId = Guid.NewGuid(),
                Payments = payments.Where(x => x.OrderId == orderIdWithExpiredPayments).ToList()
            },
            new()
            {
                Id = orderIdWithPendingPayment,
                UserId = sharedUser,
                ProductId = Guid.NewGuid(),
                Payments = payments.Where(x => x.OrderId == orderIdWithPendingPayment).ToList()
            },
            new()
            {
                Id = orderIdWithFinishedPayment,
                UserId = sharedUser,
                ProductId = Guid.NewGuid(),
                Payments = payments.Where(x => x.OrderId == orderIdWithFinishedPayment).ToList()
            }
        };

        payments.ForEach(x => x.Order = orders.First(y => y.Id == x.OrderId));

        return new(orders, payments);
    }
}