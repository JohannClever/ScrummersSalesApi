using MediatR;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
{
    public class UpdateOrderStatusCommand : IRequest<UpdateOrderResponse>
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
    }
}
