namespace OrderService.Contracts.Requests;

public class CreatePaymentReq
{
    public required Guid OrderId { get; init; }
}