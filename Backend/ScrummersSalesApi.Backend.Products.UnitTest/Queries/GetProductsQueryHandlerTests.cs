using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product; // keep if your ProductDto lives here
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Handler.Products;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.UnitTest.Queries.Products
{
    public class GetProductsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsPagedResult_WithItems()
        {
            // Arrange
            var page = 2;
            var pageSize = 5;

            var expected = new PagedResult<ProductDto>
            {
                Items = new List<ProductDto>
                {
                    new ProductDto( Id: Guid.NewGuid(), Name:"A", Description: "a", Price: 1.1m, Stock: 11,CreateDate: System.DateTime.UtcNow, UpdateDate: System.DateTime.UtcNow),
                    new ProductDto(Id:Guid.NewGuid(), Name:"B", Description:"b", Price: 2.2m, Stock: 22,CreateDate: System.DateTime.UtcNow, UpdateDate: System.DateTime.UtcNow )
                },
                Page = page,
                PageSize = pageSize,
                Total = 42
            };

            var productService = new Mock<IProductService>();
            productService
                .Setup(s => s.GetProductsAsync(page, pageSize))
                .ReturnsAsync(expected);

            var handler = new GetProductsQueryHandler(productService.Object);
            var query = new GetProductsQuery(page, pageSize);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Page, result.Page);
            Assert.Equal(expected.PageSize, result.PageSize);
            Assert.Equal(expected.Total, result.Total);

            Assert.NotNull(result.Items);
            Assert.Equal(expected.Items.Count, result.Items.Count);
            Assert.Equal(expected.Items.First().Id, result.Items.First().Id);

            productService.Verify(
                s => s.GetProductsAsync(page, pageSize),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyPagedResult_WhenNoItems()
        {
            // Arrange
            var page = 1;
            var pageSize = 20;

            var expected = new PagedResult<ProductDto>
            {
                Items = new List<ProductDto>(),
                Page = page,
                PageSize = pageSize,
                Total = 0
            };

            var productService = new Mock<IProductService>();
            productService
                .Setup(s => s.GetProductsAsync(page, pageSize))
                .ReturnsAsync(expected);

            var handler = new GetProductsQueryHandler(productService.Object);
            var query = new GetProductsQuery(page, pageSize);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Total);
            Assert.Empty(result.Items);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);

            productService.Verify(
                s => s.GetProductsAsync(page, pageSize),
                Times.Once);
        }

        [Fact]
        public async Task Handle_PassesParameters_ToService()
        {
            // Arrange
            var page = 3;
            var pageSize = 15;

            var productService = new Mock<IProductService>();
            productService
                .Setup(s => s.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedResult<ProductDto>
                {
                    Items = new List<ProductDto>(),
                    Page = page,
                    PageSize = pageSize,
                    Total = 0
                });

            var handler = new GetProductsQueryHandler(productService.Object);
            var query = new GetProductsQuery(page, pageSize);

            // Act
            _ = await handler.Handle(query, CancellationToken.None);

            // Assert
            productService.Verify(
                s => s.GetProductsAsync(page, pageSize),
                Times.Once);
        }
    }
}
