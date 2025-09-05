
namespace ScrummersSalesApi.Backend.Products.Domain.ReadModels.Products
{
    public record ProductRead(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int Stock,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
