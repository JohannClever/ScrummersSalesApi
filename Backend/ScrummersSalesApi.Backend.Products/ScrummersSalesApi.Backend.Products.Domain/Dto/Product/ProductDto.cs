namespace ScrummersSalesApi.Backend.Products.Domain.Dto.Product
{
    public record ProductDto(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        int Stock,
        DateTime CreateDate,
        DateTime UpdateDate
    );
}
