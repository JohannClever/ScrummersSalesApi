// ScrummersSalesApi.Backend.Products.Domain/Queries/Handler/Products/GetProductsByIdsQueryHandler.cs
using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Product;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Handler.Products
{
    /// <summary>
    /// Handles the GetProductsByIdsQuery by calling the ProductService.
    /// </summary>
    public sealed class GetProductsByIdsQueryHandler
        : IRequestHandler<GetProductsByIdsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductService _productService;

        public GetProductsByIdsQueryHandler(IProductService productService)
            => _productService = productService;

        public Task<IEnumerable<ProductDto>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
            => _productService.GetProductsByIdsAsync(request.Ids);
    }
}
