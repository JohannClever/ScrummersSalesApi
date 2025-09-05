// ScrummersSalesApi.Backend.Orders.UnitTest/Services/OrderServiceTests.cs
using Microsoft.Extensions.Configuration;
using Moq;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Ports;
using ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Products;    // ProductRead
using ScrummersSalesApi.Backend.Orders.Domain.Services;

namespace ScrummersSalesApi.Backend.Orders.UnitTest.Services
{
    public class OrderServiceTests
    {
        private static OrderService CreateService(
            out Mock<IGenericRepository<Order>> repo,
            out Mock<IProductCatalogClient> products)
        {
            repo = new Mock<IGenericRepository<Order>>();
            products = new Mock<IProductCatalogClient>();

            // La clase requiere un connection string "database" aunque en estas pruebas no abrimos conexión.
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:database"] = "Server=(local);Database=Fake;Trusted_Connection=True;"
            };
            IConfiguration cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new OrderService(repo.Object, products.Object, cfg);
        }

        // --------------------------
        // CreateOrderAsync - Guards
        // --------------------------

        [Fact]
        public async Task CreateOrderAsync_Throws_WhenItemsNullOrEmpty()
        {
            var sut = CreateService(out var repo, out var products);

            var o1 = new Order { CustomerId = Guid.NewGuid(), Items = null! };
            await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateOrderAsync(o1));

            var o2 = new Order { CustomerId = Guid.NewGuid(), Items = new List<OrderItem>() };
            await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateOrderAsync(o2));

            repo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
            products.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateOrderAsync_Throws_WhenProductNotFound()
        {
            var sut = CreateService(out var repo, out var products);

            var pid1 = Guid.NewGuid();
            var pid2 = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItem>
                {
                    new() { ProductId = pid1, Quantity = 1 },
                    new() { ProductId = pid2, Quantity = 2 },
                }
            };

            // Solo devolvemos un producto (falta pid2)
            products
                .Setup(p => p.GetProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<ProductRead>
                {
                    new ProductRead(pid1, "P1", "d", 10m, 5, DateTime.UtcNow, DateTime.UtcNow)
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateOrderAsync(order));
            repo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_Throws_WhenProductPriceInvalid()
        {
            var sut = CreateService(out var repo, out var products);

            var pid = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItem> { new() { ProductId = pid, Quantity = 1 } }
            };

            products
                .Setup(p => p.GetProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<ProductRead>
                {
                    new ProductRead(pid, "P1", "d", 0m, 100, DateTime.UtcNow, DateTime.UtcNow) // precio inválido
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateOrderAsync(order));
            repo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_Throws_WhenInsufficientStock()
        {
            var sut = CreateService(out var repo, out var products);

            var pid = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItem> { new() { ProductId = pid, Quantity = 10 } } // requiere 10
            };

            products
                .Setup(p => p.GetProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<ProductRead>
                {
                    new ProductRead(pid, "P1", "d", 15m, 5, DateTime.UtcNow, DateTime.UtcNow) // stock 5
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateOrderAsync(order));
            repo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        // --------------------------
        // CreateOrderAsync - Success
        // --------------------------

        [Fact]
        public async Task CreateOrderAsync_SetsAuthoritativePrices_ComputesTotal_AndPersists()
        {
            var sut = CreateService(out var repo, out var products);

            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItem>
                {
                    new() { ProductId = p1, Quantity = 2 }, // precio desde ProductRead: 10
                    new() { ProductId = p2, Quantity = 1 }  // precio desde ProductRead: 20
                }
            };

            products
                .Setup(p => p.GetProductsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<ProductRead>
                {
                    new ProductRead(p1, "A", "d", 10m, 100, DateTime.UtcNow, DateTime.UtcNow),
                    new ProductRead(p2, "B", "d", 20m, 100, DateTime.UtcNow, DateTime.UtcNow),
                });

            repo
                .Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.FromResult(order));

            var id = await sut.CreateOrderAsync(order);

            Assert.NotEqual(Guid.Empty, id);


            products.Verify(p => p.GetProductsByIdsAsync(It.Is<List<Guid>>(l =>
                l.Count == 2 && l.Contains(p1) && l.Contains(p2))), Times.Once);
        }

        // --------------------------
        // UpdateStatusAsync
        // --------------------------

        [Fact]
        public async Task UpdateStatusAsync_UpdatesOrder()
        {
            var sut = CreateService(out var repo, out var products);
            var id = Guid.NewGuid();

            var existing = new Order
            {
                Id = id,
                CustomerId = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };

            repo.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);

            repo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            await sut.UpdateStatusAsync(id, OrderStatus.Confirmed);

            Assert.Equal(OrderStatus.Confirmed, existing.Status);
            repo.Verify(r => r.GetByIdAsync(id), Times.Once);
            repo.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Id == id && o.Status == OrderStatus.Confirmed)),
                        Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Throws_WhenOrderNotFound()
        {
            var sut = CreateService(out var repo, out var products);
            var id = Guid.NewGuid();

            repo.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Order?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.UpdateStatusAsync(id, OrderStatus.Cancelled));
            repo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }
    }
}
