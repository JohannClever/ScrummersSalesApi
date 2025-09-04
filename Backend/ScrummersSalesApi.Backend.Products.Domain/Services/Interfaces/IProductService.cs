using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;

namespace ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(int page, int pageSize);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<Product> CreateProductAsync(Product input);
    }
}
