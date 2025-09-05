using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;         // record ProductDto
using ScrummersSalesApi.Backend.Products.Api.Controllers;
using ScrummersSalesApi.Backend.Products.Domain.Commands.Command;                  // CreateProductCommand / CreateProductCommandResponse
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;                      // PagedResult<T>
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;            // GetProductsQuery / GetProductByIdQuery

namespace ScrummersSalesApi.Backend.Products.UnitTest.Controllers
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task Get_ReturnsPagedResult_And_SendsQueryToMediator()
        {
            // Arrange
            var page = 2;
            var pageSize = 5;

            var p1 = new ProductDto(
                Guid.NewGuid(), "A", "a", 1.1m, 11, DateTime.UtcNow, DateTime.UtcNow);
            var p2 = new ProductDto(
                Guid.NewGuid(), "B", "b", 2.2m, 22, DateTime.UtcNow, DateTime.UtcNow);

            var expected = new PagedResult<ProductDto>
            {
                Items = new List<ProductDto> { p1, p2 },
                Page = page,
                PageSize = pageSize,
                Total = 40
            };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(It.Is<GetProductsQuery>(q => q.Page == page && q.PageSize == pageSize),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<ProductsController>>();
            var controller = new ProductsController(logger, mediator.Object);

            // Act
            var result = await controller.Get(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Page, result.Page);
            Assert.Equal(expected.PageSize, result.PageSize);
            Assert.Equal(expected.Total, result.Total);
            Assert.Collection(result.Items,
                i => Assert.Equal(p1, i),
                i => Assert.Equal(p2, i));

            mediator.Verify(m => m.Send(It.Is<GetProductsQuery>(q => q.Page == page && q.PageSize == pageSize),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsProductDto_And_SendsQueryToMediator()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expected = new ProductDto(
                productId, "Sample", "Desc", 9.99m, 5, DateTime.UtcNow, DateTime.UtcNow);

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<ProductsController>>();
            var controller = new ProductsController(logger, mediator.Object);

            // Act
            var result = await controller.GetById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result); // record equality

            mediator.Verify(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithMediatorResponse()
        {
            // Arrange
            var createdId = Guid.NewGuid();

            var request = new CreateProductCommand
            {
                Name = "New Product",
                Description = "Desc",
                Price = 12.34m,
                Stock = 3
            };

            var response = new CreateProductCommandResponse
            {
                ProductId = createdId
            };

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var logger = Mock.Of<ILogger<ProductsController>>();
            var controller = new ProductsController(logger, mediator.Object);

            // Act
            var actionResult = await controller.Create(request, CancellationToken.None);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(ProductsController.GetById), createdAt.ActionName);
            Assert.NotNull(createdAt.RouteValues);
            Assert.True(createdAt.RouteValues!.ContainsKey("id"));
            Assert.Equal(createdId, createdAt.RouteValues["id"]);

            var value = Assert.IsType<CreateProductCommandResponse>(createdAt.Value);
            Assert.Equal(createdId, value.ProductId);

            mediator.Verify(m => m.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
