
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.Domain.Queries.Handler.Products
{
    public sealed class GetOrdersQueryHandler: IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderService _orderService;

        public GetOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            return _orderService.GetOrdersAsync(request.Page, request.PageSize);
        }
    }
}
