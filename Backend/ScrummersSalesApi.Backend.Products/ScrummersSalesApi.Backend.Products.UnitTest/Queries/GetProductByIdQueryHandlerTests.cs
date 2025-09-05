using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product; // Adjust if your ProductDto lives elsewhere
using ScrummersSalesApi.Backend.Products.Domain.Queries.Handler.Products;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.UnitTest.Queries.Products
{
    public class GetProductByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenProductExists_ReturnsProductDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new ProductDto(
                Id: id,
                Name: "Test Product",
                Description: "Desc",
                Price: 10.5m,
                Stock: 5,
                CreateDate: System.DateTime.UtcNow,
                UpdateDate: System.DateTime.UtcNow
            );

            var service = new Mock<IProductService>();
            service.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(expected);

            var handler = new GetProductByIdQueryHandler(service.Object);
            var query = new GetProductByIdQuery(id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Id, result!.Id);
            Assert.Equal(expected.Name, result.Name);
            service.Verify(s => s.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var service = new Mock<IProductService>();
            service.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
                   .ReturnsAsync((ProductDto?)null);

            var handler = new GetProductByIdQueryHandler(service.Object);
            var query = new GetProductByIdQuery(id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            service.Verify(s => s.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
