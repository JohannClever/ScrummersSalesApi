namespace ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Products
{
    public record ProductRead(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        int Stock,
        DateTime CreateDate,
        DateTime UpdateDate
    );
}