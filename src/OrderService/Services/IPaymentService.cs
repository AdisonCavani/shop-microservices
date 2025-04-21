using OrderService.Contracts.Dtos;
using ProtobufSpec.Events;

namespace OrderService.Services;

public interface IPaymentService
{
    Task<Tuple<Guid, PaymentDto>?> CreatePaymentAsync(Guid orderId, Guid userId, string userEmail);

    Task<PaymentDto?> GetPaymentAsync(Guid orderId, Guid userId);

    Task PaymentReceivedAsync(PaymentSucceededEvent paymentEvent);
}