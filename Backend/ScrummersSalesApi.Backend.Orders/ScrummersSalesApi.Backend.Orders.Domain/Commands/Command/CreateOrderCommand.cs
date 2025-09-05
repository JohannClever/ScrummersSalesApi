using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
{
    // ScrummersSalesApi.Backend.Orders.Domain/Commands/Command/CreateOrderCommand.cs
    using MediatR;
    using System.ComponentModel.DataAnnotations;

    namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
    {
        /// <summary>
        /// Command to create a new order.
        /// </summary>
        public class CreateOrderCommand : IRequest<CreateOrderResponse>
        {
            [Required]
            public Guid CustomerId { get; set; }

            [Required]
            public List<OrderItemCommand> Items { get; set; } = new();
        }

        /// <summary>
        /// Represents a single order item in the CreateOrderCommand.
        /// </summary>
        public class OrderItemCommand
        {
            [Required]
            public Guid ProductId { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
            public int Quantity { get; set; }
        }
    }

}
