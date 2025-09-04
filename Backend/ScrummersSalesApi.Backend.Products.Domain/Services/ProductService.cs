using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using System.Text;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;
using ScrummersSalesApi.Backend.Products.Domain.Ports;
using ScrummersSalesApi.Backend.Products.Domain.Services.Generic;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.Domain.Services
{
    [DomainService]
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly string? _connectionString;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IConfiguration configuration)
        {
            _productRepository = productRepository;

            _connectionString = configuration.GetConnectionString("database");
        }

        /// <summary>
        /// Devuelve un listado paginado de productos usando Dapper.
        /// </summary>
        public async Task<PagedResult<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var offset = (page - 1) * pageSize;

            // Un roundtrip: datos + total
            var sql = new StringBuilder();
            sql.AppendLine("SELECT id, name, description, price, stock, createdate, updatedate");
            sql.AppendLine("FROM Products");
            sql.AppendLine("ORDER BY id");
            sql.AppendLine("OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;");
            sql.AppendLine();
            sql.AppendLine("SELECT COUNT(*) FROM products;");

            using var connection = new SqlConnection(_connectionString);

            using var multi = await connection.QueryMultipleAsync(
                sql.ToString(),
                new { offset, pageSize });

            var items = (await multi.ReadAsync<ProductDto>()).ToList();
            var total = await multi.ReadFirstAsync<int>();

            return new PagedResult<ProductDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }


        /// <summary>
        /// Devuelve un producto por Id usando Dapper.
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            const string sql = @"
                SELECT id, name, description, price, stock, createdate, updatedate
                FROM products
                WHERE id = @id;";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new { id });
        }

        /// <summary>
        /// Crea un producto con EF (repositorio genérico).
        /// </summary>
        public async Task<Product> CreateProductAsync(Product input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.Price <= 0) throw new ArgumentOutOfRangeException(nameof(input.Price), "Price must be > 0");
            if (input.Stock < 0) throw new ArgumentOutOfRangeException(nameof(input.Stock), "Stock cannot be negative");

            input.CreateDate = DateTime.UtcNow;
            input.UpdatedDate = input.CreateDate;

            await _productRepository.AddAsync(input);

            return input;
        }
    }
}
