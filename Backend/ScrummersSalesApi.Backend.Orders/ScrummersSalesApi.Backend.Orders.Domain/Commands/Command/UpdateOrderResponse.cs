// ScrummersSalesApi.Backend.Orders.Domain/Commands/Command/CreateOrderResponse.cs
namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
{
    /// <summary>
    /// Response returned when a new order is created.
    /// </summary>
    public class UpdateOrderResponse
    {
        public Guid OrderId { get; set; }
        public bool Updated { get; set; }

        public UpdateOrderResponse(Guid orderId, bool updated)
        {
            OrderId = orderId;
            Updated = updated;
        }
    }
}
