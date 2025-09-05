using ScrummersSalesApi.Backend.Orders.Domain.Entities.Generic;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;


// ScrummersSalesApi.Backend.Orders.Domain/Entities/Order.cs
namespace ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders
{
    public class Order : EntityBase<Guid>
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem : EntityBase<Guid>
    {
        public OrderItem()
        {
            Id = Guid.NewGuid();
        }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }          // integrates with Product MS (Guid)
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }


}
