// ScrummersSalesApi.Backend.Orders.UnitTest/Controllers/OrdersControllerTests.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ScrummersSalesApi.Backend.Orders.Api.Controllers;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command.ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Controllers
{
    public class OrdersControllerTests
    {
        private static OrdersController CreateController(
            out Mock<IMediator> mediatorMock)
        {
            mediatorMock = new Mock<IMediator>();
            var logger = Mock.Of<ILogger<OrdersController>>();
            return new OrdersController(mediatorMock.Object, logger);
        }

        [Fact]
        public async Task Get_ReturnsPagedOrders_AndSendsQuery()
        {
            // Arrange
            var controller = CreateController(out var mediator);
            var page = 2;
            var pageSize = 5;

            var expected = new PagedResult<OrderDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = 12,
                Items = new List<OrderDto>
                {
                    new(Guid.NewGuid(), Guid.NewGuid(), "Confirmed", 100m, DateTime.UtcNow, Array.Empty<OrderItemDto>())
                }
            };

            mediator
                .Setup(m => m.Send(It.Is<GetOrdersQuery>(q => q.Page == page && q.PageSize == pageSize),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var result = await controller.Get(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Total, result.Total);
            Assert.Equal(expected.Page, result.Page);
            Assert.Equal(expected.PageSize, result.PageSize);
            Assert.Single(result.Items);
            mediator.Verify(m => m.Send(It.Is<GetOrdersQuery>(q => q.Page == page && q.PageSize == pageSize),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsOrder_AndSendsQuery()
        {
            // Arrange
            var controller = CreateController(out var mediator);
            var id = Guid.NewGuid();

            var dto = new OrderDto(
                id,
                Guid.NewGuid(),
                "Pending",
                55.5m,
                DateTime.UtcNow,
                new List<OrderItemDto> { new(Guid.NewGuid(), Guid.NewGuid(),"P1",2, 10m) });

            mediator
                .Setup(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await controller.GetById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            mediator.Verify(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == id),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithResponseBody()
        {
            // Arrange
            var controller = CreateController(out var mediator);

            var cmd = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemCommand>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 3 }
                }
            };

            var created = new CreateOrderResponse(Guid.NewGuid());

            mediator
                .Setup(m => m.Send(It.Is<CreateOrderCommand>(c => c.CustomerId == cmd.CustomerId && c.Items.Count == 1),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            // Act
            var action = await controller.Create(cmd, CancellationToken.None);

            // Assert
            var result = Assert.IsType<CreatedAtActionResult>(action.Result);
            Assert.Equal(nameof(OrdersController.GetById), result.ActionName);
            Assert.NotNull(result.RouteValues);
            Assert.True(result.RouteValues!.ContainsKey("id"));
            Assert.Equal(created.OrderId, (Guid)result.RouteValues["id"]!);

            var body = Assert.IsType<CreateOrderResponse>(result.Value);
            Assert.Equal(created.OrderId, body.OrderId);

            mediator.Verify(m => m.Send(It.Is<CreateOrderCommand>(c => c.CustomerId == cmd.CustomerId),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_WhenUpdatedTrue_ReturnsOk_WithPayload()
        {
            // Arrange
            var controller = CreateController(out var mediator);
            var id = Guid.NewGuid();
            var input = new UpdateOrderStatusRequest { Status = OrderStatus.Confirmed };
            var expected = new UpdateOrderResponse(id, true);

            mediator
                .Setup(m => m.Send(It.Is<UpdateOrderStatusCommand>(c => c.Id == id && c.Status == input.Status),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var action = await controller.UpdateStatus(id, input, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            var body = Assert.IsType<UpdateOrderResponse>(ok.Value);
            Assert.Equal(id, body.OrderId);
            Assert.True(body.Updated);

            mediator.Verify(m => m.Send(It.Is<UpdateOrderStatusCommand>(c => c.Id == id && c.Status == input.Status),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_WhenUpdatedFalse_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController(out var mediator);
            var id = Guid.NewGuid();
            var input = new UpdateOrderStatusRequest { Status = OrderStatus.Cancelled };
            var expected = new UpdateOrderResponse(id, false);

            mediator
                .Setup(m => m.Send(It.Is<UpdateOrderStatusCommand>(c => c.Id == id && c.Status == input.Status),
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var action = await controller.UpdateStatus(id, input, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            var body = Assert.IsType<UpdateOrderResponse>(bad.Value);
            Assert.Equal(id, body.OrderId);
            Assert.False(body.Updated);

            mediator.Verify(m => m.Send(It.Is<UpdateOrderStatusCommand>(c => c.Id == id && c.Status == input.Status),
                                        It.IsAny<CancellationToken>()),
                            Times.Once);
        }
    }
}
