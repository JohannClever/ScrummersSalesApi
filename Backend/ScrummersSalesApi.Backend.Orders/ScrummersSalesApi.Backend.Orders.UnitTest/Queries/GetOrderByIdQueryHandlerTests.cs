// ScrummersSalesApi.Backend.Orders.UnitTest/Queries/GetOrderByIdQueryHandlerTests.cs
using Moq;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Handler.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenOrderExists_ReturnsOrderDto()
        {
            // Arrange
            var id = Guid.NewGuid();

            var expected = new OrderDto(
                Id: id,
                CustomerId:Guid.NewGuid(),
                Status: "Confirmed",
                TotalAmount: 199.99m,
                OrderDate: DateTime.UtcNow,
                Items: new List<OrderItemDto>
                {
                    new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(),"P1" ,2, 49.99m),
                    new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(),"P2" ,1, 99.99m)
                });

            var service = new Mock<IOrderService>();
            service.Setup(s => s.GetOrderByIdAsync(id))
                   .ReturnsAsync(expected);

            var handler = new GetOrderByIdQueryHandler(service.Object);
            var query = new GetOrderByIdQuery(id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Id, result!.Id);
            Assert.Equal(expected.CustomerId, result.CustomerId);
            Assert.Equal(expected.Status, result.Status);
            Assert.Equal(expected.TotalAmount, result.TotalAmount);
            service.Verify(s => s.GetOrderByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenOrderDoesNotExist_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            var service = new Mock<IOrderService>();
            service.Setup(s => s.GetOrderByIdAsync(id))
                   .ReturnsAsync((OrderDto?)null);

            var handler = new GetOrderByIdQueryHandler(service.Object);
            var query = new GetOrderByIdQuery(id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            service.Verify(s => s.GetOrderByIdAsync(id), Times.Once);
        }
    }
}
