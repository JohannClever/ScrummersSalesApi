using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Ports;
using ScrummersSalesApi.Backend.Products.Domain.Services;
using Xunit;

namespace ScrummersSalesApi.Backend.Products.UnitTest.Services
{
    public class ProductServiceCreateTests
    {
        private static ProductService CreateService(
            Mock<IGenericRepository<Product>> repoMock,
            string connectionString = "Server=.;Database=Fake;Trusted_Connection=True;")
        {
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new ProductService(repoMock.Object, config);
        }

        [Fact]
        public async Task CreateProductAsync_Throws_WhenInputIsNull()
        {
            // Arrange
            var repo = new Mock<IGenericRepository<Product>>();
            var sut = CreateService(repo);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CreateProductAsync(null!));
            repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]      // zero
        [InlineData(-10.0)] // negative
        public async Task CreateProductAsync_Throws_WhenPriceIsNotPositive(decimal price)
        {
            // Arrange
            var repo = new Mock<IGenericRepository<Product>>();
            var sut = CreateService(repo);

            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = "X",
                Description = "desc",
                Price = price,     // invalid
                Stock = 1          // valid
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.CreateProductAsync(p));
            repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task CreateProductAsync_Throws_WhenStockIsNegative(int stock)
        {
            // Arrange
            var repo = new Mock<IGenericRepository<Product>>();
            var sut = CreateService(repo);

            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = "X",
                Description = "desc",
                Price = 10.5m,   // valid
                Stock = stock    // invalid
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.CreateProductAsync(p));
            repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task CreateProductAsync_SetsTimestamps_AndCallsRepository()
        {
            // Arrange
            var repo = new Mock<IGenericRepository<Product>>();
            repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns(Task.FromResult(new Product() { }));

            var sut = CreateService(repo);

            var before = DateTime.UtcNow;
            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = "X",
                Description = "desc",
                Price = 10.5m,
                Stock = 3
            };

            // Act
            var result = await sut.CreateProductAsync(p);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(p, result);

            // Timestamps should be set within [before, after]
            Assert.InRange(result.CreateDate, before, after);
            Assert.InRange(result.UpdatedDate, before, after);
            Assert.Equal(result.CreateDate, result.UpdatedDate);

            repo.Verify(r => r.AddAsync(It.Is<Product>(x => x == p)), Times.Once);
        }
    }
}
