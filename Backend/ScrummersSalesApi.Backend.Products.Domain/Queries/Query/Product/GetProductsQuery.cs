// ScrummersSalesApi.Backend.Products.Domain/Queries/Query/Products/GetProductsQuery.cs
using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products
{
    public sealed class GetProductsQuery : IRequest<PagedResult<ProductDto>>
    {
        public int Page { get; }
        public int PageSize { get; }

        public GetProductsQuery(int page, int pageSize)
        {
            Page = page <= 0 ? 1 : page;
            PageSize = pageSize <= 0 ? 20 : pageSize;
        }
    }
}
