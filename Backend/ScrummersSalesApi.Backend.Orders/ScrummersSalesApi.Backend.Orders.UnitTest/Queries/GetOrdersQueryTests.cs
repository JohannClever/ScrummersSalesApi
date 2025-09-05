// ScrummersSalesApi.Backend.Orders.UnitTest/Queries/GetOrdersQueryTests.cs
using Moq;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Handler.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Queries
{
    public class GetOrdersQueryTests
    {
        [Theory]
        [InlineData(0, 0, 1, 20)]
        [InlineData(-5, -10, 1, 20)]
        [InlineData(1, 50, 1, 50)]
        [InlineData(3, 10, 3, 10)]
        public void Query_NormalizesValues(int inputPage, int inputPageSize, int expectedPage, int expectedPageSize)
        {
            // Act
            var query = new GetOrdersQuery(inputPage, inputPageSize);

            // Assert
            Assert.Equal(expectedPage, query.Page);
            Assert.Equal(expectedPageSize, query.PageSize);
        }

        [Fact]
        public async Task Handler_ReturnsResult_FromService_AndCallsWithCorrectArgs()
        {
            // Arrange
            var page = 2;
            var pageSize = 5;
            var query = new GetOrdersQuery(page, pageSize);

            var expected = new PagedResult<OrderDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = 42,
                Items = new List<OrderDto>
                {
                    new(
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        "Confirmed",
                        123.45m,
                        DateTime.UtcNow,
                        new List<OrderItemDto>())
                }
            };

            var svc = new Mock<IOrderService>();
            svc.Setup(s => s.GetOrdersAsync(page, pageSize))
               .ReturnsAsync(expected);

            var handler = new GetOrdersQueryHandler(svc.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Total, result.Total);
            Assert.Equal(expected.Page, result.Page);
            Assert.Equal(expected.PageSize, result.PageSize);
            Assert.Single(result.Items);

            svc.Verify(s => s.GetOrdersAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task Handler_UsesNormalizedValues_WhenInputIsInvalid()
        {
            // Arrange
            var query = new GetOrdersQuery(0, -10); // should normalize to page=1, pageSize=20

            var expected = new PagedResult<OrderDto> { Page = 1, PageSize = 20, Total = 0, Items = Array.Empty<OrderDto>() };

            var svc = new Mock<IOrderService>();
            svc.Setup(s => s.GetOrdersAsync(1, 20))
               .ReturnsAsync(expected);

            var handler = new GetOrdersQueryHandler(svc.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            svc.Verify(s => s.GetOrdersAsync(1, 20), Times.Once);
        }
    }
}
