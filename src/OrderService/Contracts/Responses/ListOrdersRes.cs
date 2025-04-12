using OrderService.Contracts.Dtos;

namespace OrderService.Contracts.Responses;

public class ListOrdersRes
{
    public IEnumerable<OrderDto> Orders { get; set; }
}