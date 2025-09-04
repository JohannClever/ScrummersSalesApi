// ScrummersSalesApi.Backend.Products.Domain/Queries/Handler/Products/GetProductByIdQueryHandler.cs
using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Handler.Products
{
    public sealed class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductService _productService;

        public GetProductByIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return _productService.GetProductByIdAsync(request.Id);
        }
    }
}
