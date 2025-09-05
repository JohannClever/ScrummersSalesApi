// ScrummersSalesApi.Backend.Orders.Domain/Queries/Handler/Products/GetProductByIdQueryHandler.cs
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.Domain.Queries.Handler.Products
{
    public sealed class GetOrderByIdQueryHandler
        : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _orderService.GetOrderByIdAsync(request.Id);
        }
    }
}
