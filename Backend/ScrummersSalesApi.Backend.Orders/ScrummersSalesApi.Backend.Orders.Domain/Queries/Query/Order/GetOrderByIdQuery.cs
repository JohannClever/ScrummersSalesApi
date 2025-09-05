// ScrummersSalesApi.Backend.Orders.Domain/Queries/Query/Products/GetProductByIdQuery.cs
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;

namespace ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products
{
    public sealed class GetOrderByIdQuery : IRequest<OrderDto?>
    {
        public Guid Id { get; }

        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
