// ScrummersSalesApi.Backend.Orders.Domain/Commands/Handler/CreateOrderCommandHandler.cs
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command.ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Handler
{
    /// <summary>
    /// Handles order creation using EF Core (write model).
    /// Validates against Product service inside IOrderService.
    /// </summary>
    public sealed class CreateOrderCommandHandler
        : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
            => _orderService = orderService;

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var order = new Order
            {
                CustomerId = request.CustomerId,
                Status = OrderStatus.Confirmed,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList()
            };

            var orderId = await _orderService.CreateOrderAsync(order);
            return  new CreateOrderResponse(orderId);
        }
    }
}
