using ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Products;

namespace ScrummersSalesApi.Backend.Orders.Domain.Ports
{
    public interface IProductCatalogClient
    {
        Task<IEnumerable<ProductRead>> GetProductsByIdsAsync(List<Guid> productIds);
    }
}
