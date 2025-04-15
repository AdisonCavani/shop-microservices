namespace OrderService.Contracts.Requests;

public class CreateOrderReq
{
    public required Guid ProductId { get; init; }
}