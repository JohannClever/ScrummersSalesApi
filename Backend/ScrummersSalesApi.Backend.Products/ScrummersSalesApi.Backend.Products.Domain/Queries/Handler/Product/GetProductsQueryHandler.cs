
using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Handler.Products
{
    public sealed class GetProductsQueryHandler: IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return _productService.GetProductsAsync(request.Page, request.PageSize);
        }
    }
}
