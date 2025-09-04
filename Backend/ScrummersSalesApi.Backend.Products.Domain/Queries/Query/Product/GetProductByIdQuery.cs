// ScrummersSalesApi.Backend.Products.Domain/Queries/Query/Products/GetProductByIdQuery.cs
using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products
{
    public sealed class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public Guid Id { get; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
