// ScrummersSalesApi.Backend.Orders.Domain/Queries/Query/Products/GetProductsQuery.cs
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;

namespace ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products
{
    public sealed class GetOrdersQuery : IRequest<PagedResult<OrderDto>>
    {
        public int Page { get; }
        public int PageSize { get; }

        public GetOrdersQuery(int page, int pageSize)
        {
            Page = page <= 0 ? 1 : page;
            PageSize = pageSize <= 0 ? 20 : pageSize;
        }
    }
}
