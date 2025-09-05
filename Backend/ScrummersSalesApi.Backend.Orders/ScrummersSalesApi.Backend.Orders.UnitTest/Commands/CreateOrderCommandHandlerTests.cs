// ScrummersSalesApi.Backend.Orders.UnitTest/Commands/CreateOrderCommandHandlerTests.cs
using Moq;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command.ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Handler;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_BuildsOrderAndCallsService_ReturnsOrderId()
        {
            // Arrange
            var cmd = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemCommand>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 },
                    new() { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            var expectedOrderId = Guid.NewGuid();

            var service = new Mock<IOrderService>();
            service
                .Setup(s => s.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(expectedOrderId);

            var handler = new CreateOrderCommandHandler(service.Object);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert: response
            Assert.NotNull(result);
            Assert.Equal(expectedOrderId, result.OrderId);

            // Assert: mapping of the Order passed to the service
            service.Verify(s => s.CreateOrderAsync(It.Is<Order>(o =>
                   o.CustomerId == cmd.CustomerId
                && o.Status == OrderStatus.Confirmed
                && o.Items.Count == 2
                && o.Items[0].ProductId == cmd.Items[0].ProductId
                && o.Items[0].Quantity == cmd.Items[0].Quantity
                && o.Items[1].ProductId == cmd.Items[1].ProductId
                && o.Items[1].Quantity == cmd.Items[1].Quantity
            )), Times.Once);
        }
    }
}
