using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ScrummersSalesApi.Backend.Orders.Domain.Commons.Exception;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Ports;
using ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Products;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Generic;
using ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces;
using System.Text;

namespace ScrummersSalesApi.Backend.Orders.Domain.Services
{
    [DomainService]
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _repo;
        private readonly IProductCatalogClient _products;
        private readonly string _conn;

        public OrderService(IGenericRepository<Order> repo, IProductCatalogClient products, IConfiguration cfg)
        {
            _repo = repo;
            _products = products;
            _conn = cfg.GetConnectionString("database")
                    ?? throw new InvalidOperationException("ConnectionStrings:database is missing.");
        }



        public async Task<PagedResult<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var offset = (page - 1) * pageSize;

            var sql = new StringBuilder();
            sql.AppendLine("""
                SELECT o.Id, o.CustomerId, o.Status, o.TotalAmount, o.OrderDate,
                       oi.Id AS ItemId, oi.ProductId, oi.Quantity, oi.UnitPrice
                FROM dbo.Orders o
                LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.Id
                ORDER BY o.OrderDate DESC, o.Id
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

                SELECT COUNT(*) FROM dbo.Orders;
            """);

            using var cn = new SqlConnection(_conn);
            using var multi = await cn.QueryMultipleAsync(sql.ToString(), new { offset, pageSize });

            var rows = (await multi.ReadAsync<OrderRead>()).ToList();
            var total = await multi.ReadFirstAsync<int>();


            var productsIds = rows
                             .Where(i => i.ProductId.HasValue)   // solo los que tienen valor
                             .Select(i => i.ProductId.Value)     // tomar el Guid
                             .Distinct()
                             .ToList();

            Dictionary<Guid, ProductRead> productsById = await GetProductsAsync(productsIds);

            var headers = rows
                .GroupBy(r => new { r.Id, r.CustomerId, r.Status, r.TotalAmount, r.OrderDate })
                .Select(g =>
                {
                    var items = g.Where(x => x.ItemId != null)
                                 .Select(x => new OrderItemDto(x.ItemId!.Value, x.ProductId!.Value, GetProductName(x.ProductId!.Value,productsById), x.Quantity!.Value, x.UnitPrice!.Value))
                                 .ToList()
                                 .AsReadOnly();

                    return new OrderDto(g.Key.Id, g.Key.CustomerId, g.Key.Status.ToOrderStatusString(), g.Key.TotalAmount, g.Key.OrderDate, items);
                })
                .ToList();

            return new PagedResult<OrderDto> { Items = headers, Page = page, PageSize = pageSize, Total = total };
        }



        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            const string sql = """
                SELECT o.Id, o.CustomerId, o.Status, o.TotalAmount, o.OrderDate,
                       oi.Id AS ItemId, oi.ProductId, oi.Quantity, oi.UnitPrice
                FROM dbo.Orders o
                LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.Id
                WHERE o.Id = @id
                ORDER BY oi.Id;
            """;

            using var cn = new SqlConnection(_conn);
            var rows = (await cn.QueryAsync<OrderRead>(sql, new { id })).ToList();
            if (rows.Count == 0) return null;

            var h = rows.First();

            var productsIds = rows
                 .Where(i => i.ProductId.HasValue)   // solo los que tienen valor
                 .Select(i => i.ProductId.Value)     // tomar el Guid
                 .Distinct()
                 .ToList();

            Dictionary<Guid, ProductRead> productsById = await GetProductsAsync(productsIds);

            var items = rows.Where(r => r.ItemId != null)
                            .Select(r => new OrderItemDto(r.ItemId!.Value, r.ProductId!.Value, GetProductName(r.ProductId!.Value, productsById), r.Quantity!.Value, r.UnitPrice!.Value))
                            .ToList()
                            .AsReadOnly();

            return new OrderDto(h.Id, h.CustomerId, h.Status.ToOrderStatusString(), h.TotalAmount, h.OrderDate, items);
        }

        // --- EF Core: entities para escrituras ---
        public async Task<Guid> CreateOrderAsync(Order model)
        {
            // Basic guard
            if (model.Items == null || model.Items.Count == 0)
                throw new ArgumentException("At least one item is required.");

            var productsIds = model.Items
                            .Select(i => i.ProductId)     // tomar el Guid
                            .Distinct()
                            .ToList();

            Dictionary<Guid, ProductRead> productsById = await GetProductsAsync(productsIds);

            // 3) Validate each requested item against the product info and set UnitPrice from the catalog (don’t trust client price)
            foreach (var item in model.Items)
            {
                if (!productsById.TryGetValue(item.ProductId, out ProductRead product))
                    throw new InvalidOperationException($"Product {item.ProductId} not found.");

                if (product.Price <= 0)
                    throw new InvalidOperationException($"Invalid price for product {item.ProductId}.");

                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}. Requested: {item.Quantity}, Available: {product.Stock}");

                // Always use the authoritative price from Products service
                item.UnitPrice = product.Price;
            }

            // 4) Build the order entity using authoritative prices
            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = OrderStatus.Confirmed,
                Items = model.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            // 5) Compute total
            order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

            // 6) Persist
            await _repo.AddAsync(order);

            return order.Id;
        }



        private string GetProductName( Guid ProductId , Dictionary<Guid, ProductRead> productsById)
        {
            if(!productsById.TryGetValue(ProductId, out ProductRead product))
              return string.Empty;

            return product.Name;
        }
        private async Task<Dictionary<Guid, ProductRead>> GetProductsAsync(List<Guid> ids)
        {

            var products = await _products.GetProductsByIdsAsync(ids); // returns IEnumerable<ProductRead> (Id, Price, Stock, ...)
            var notNullProducts = products.Where(i => i != null).ToList();

            var productsById = products.ToDictionary(p => p.Id);

            return productsById;
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus status)
        {
            var order = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Order not found.");
            order.Status = status;
            await _repo.UpdateAsync(order);
        }
    }
}
