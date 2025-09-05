namespace ScrummersSalesApi.Backend.Orders.Domain.Dto
{
    public record OrderItemDto(Guid Id, Guid ProductId, string productName, int Quantity, decimal UnitPrice);
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        string Status,
        decimal TotalAmount,
        DateTime OrderDate,
        IReadOnlyList<OrderItemDto> Items
    );
}
