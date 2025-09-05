// ScrummersSalesApi.Backend.Orders.Domain/Commands/Command/CreateOrderResponse.cs
namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
{
    /// <summary>
    /// Response returned when a new order is created.
    /// </summary>
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }

        public CreateOrderResponse(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
