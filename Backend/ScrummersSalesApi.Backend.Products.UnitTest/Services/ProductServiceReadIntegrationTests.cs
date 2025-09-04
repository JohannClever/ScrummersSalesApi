using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Ports;
using ScrummersSalesApi.Backend.Products.Domain.Services;
using Xunit;

namespace ScrummersSalesApi.Backend.Products.UnitTest.Services
{
    public class ProductServiceReadIntegrationTests
    {
        private static ProductService CreateService(string realConnectionString)
        {
            var repo = new Mock<IGenericRepository<Product>>(); // not used in read tests
            var settings = new System.Collections.Generic.Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = realConnectionString
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new ProductService(repo.Object, config);
        }

        [Fact(Skip = "Provide a real SQL Server and unskip")]
        public async Task GetProductsAsync_ReturnsPagedResult_FromRealDb()
        {
            // Arrange
            var conn = "Server=(localdb)\\MSSQLLocalDB;Database=YourTestDb;Trusted_Connection=True;";
            var sut = CreateService(conn);

            // Act
            var page = 1; var pageSize = 10;
            var result = await sut.GetProductsAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.True(result.Total >= 0);
            // Optionally assert result.Items.Count <= pageSize, etc.
        }

        [Fact(Skip = "Provide a real SQL Server and unskip")]
        public async Task GetProductByIdAsync_ReturnsProduct_FromRealDb()
        {
            // Arrange
            var conn = "Server=(localdb)\\MSSQLLocalDB;Database=YourTestDb;Trusted_Connection=True;";
            var sut = CreateService(conn);

            // Act
            var anyExistingId = Guid.Parse("PUT-SOME-EXISTING-ID-HERE");
            ProductDto? product = await sut.GetProductByIdAsync(anyExistingId);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(anyExistingId, product!.Id);
        }
    }
}
