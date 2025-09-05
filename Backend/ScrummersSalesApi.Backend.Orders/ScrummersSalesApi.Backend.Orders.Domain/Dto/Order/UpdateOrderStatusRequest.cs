using ScrummersSalesApi.Backend.Orders.Domain.Enums;

namespace ScrummersSalesApi.Backend.Orders.Domain.Dto.Order
{

    /// <summary>
    /// Request body used by PUT /api/orders/{id}/status
    /// </summary>
    public sealed class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}
