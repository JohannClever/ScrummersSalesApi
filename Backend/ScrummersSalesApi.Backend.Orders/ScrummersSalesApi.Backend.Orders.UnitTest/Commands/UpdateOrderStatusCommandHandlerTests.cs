// ScrummersSalesApi.Backend.Orders.UnitTest/Commands/UpdateOrderStatusCommandHandlerTests.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Handler;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Commands
{
    public class UpdateOrderStatusCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WhenServiceSucceeds_ReturnsUpdatedTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cmd = new UpdateOrderStatusCommand { Id = orderId, Status = OrderStatus.Confirmed };

            var service = new Mock<IOrderService>();
            // No setup needed if method just completes successfully
            var handler = new UpdateOrderStatusCommandHandler(service.Object);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.True(result.Updated);

            service.Verify(s => s.UpdateStatusAsync(orderId, OrderStatus.Confirmed), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceThrows_ReturnsUpdatedFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cmd = new UpdateOrderStatusCommand { Id = orderId, Status = OrderStatus.Cancelled };

            var service = new Mock<IOrderService>();
            service
                .Setup(s => s.UpdateStatusAsync(orderId, OrderStatus.Cancelled))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var handler = new UpdateOrderStatusCommandHandler(service.Object);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.False(result.Updated);

            service.Verify(s => s.UpdateStatusAsync(orderId, OrderStatus.Cancelled), Times.Once);
        }
    }
}
