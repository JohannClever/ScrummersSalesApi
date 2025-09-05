// ScrummersSalesApi.Backend.Orders.Domain/Commands/Handler/UpdateOrderStatusCommandHandler.cs
using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Handler
{
    /// <summary>
    /// Handles the command to update the status of an existing order.
    /// </summary>
    public sealed class UpdateOrderStatusCommandHandler: IRequestHandler<UpdateOrderStatusCommand, UpdateOrderResponse>
    {
        private readonly IOrderService _orderService;

        public UpdateOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Executes the update order status command.
        /// </summary>
        public async Task<UpdateOrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Delegates the update operation to the order service
                await _orderService.UpdateStatusAsync(request.Id, request.Status);

                return new UpdateOrderResponse(request.Id, true);
            }
            catch (Exception)
            {
                return new UpdateOrderResponse(request.Id, false);
            }
        }
    }
}
