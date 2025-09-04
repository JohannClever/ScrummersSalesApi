using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Products.Domain.Commands.Handler;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.UnitTest.Commands
{
    public class CreateProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesProduct_AndReturnsResponseWithProductId()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Sample description",
                Price = 10.5m,
                Stock = 5
            };

            var expectedId = Guid.NewGuid();
            var createdProduct = new Product
            {
                Id = expectedId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock
            };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock
                .Setup(s => s.CreateProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(createdProduct);

            var handler = new CreateProductCommandHandler(productServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedId, result.ProductId);

            productServiceMock.Verify(s =>
                s.CreateProductAsync(It.Is<Product>(p =>
                    p.Name == command.Name &&
                    p.Description == command.Description &&
                    p.Price == command.Price &&
                    p.Stock == command.Stock)),
                Times.Once);
        }
    }
}
