using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess.SeedData
{
    public static class DbSeeder
    {
        public async static void SeedData(OrdersDbContext dbContext)
        {
            bool isTableExists = dbContext.TableExists("Orders");
            if (isTableExists)
            {
                var orders = dbContext.Set<Order>().ToList();
                if (!orders.Any())
                {
                    var p1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
                    var p2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

                    var order1 = new Order
                    {
                        CustomerId = Guid.NewGuid(),
                        Status = OrderStatus.Confirmed,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = p1Id,
                                Quantity = 2,
                                UnitPrice = 100m
                            },
                            new OrderItem
                            {
                                ProductId =p2Id,
                                Quantity = 1,
                                UnitPrice = 200m
                            }
                        }
                    };
                    order1.TotalAmount = order1.Items.Sum(i => i.UnitPrice * i.Quantity);

                    var order2 = new Order
                    {
                        CustomerId = Guid.NewGuid(),
                        Status = OrderStatus.Pending,
                        OrderDate = DateTime.UtcNow.AddDays(-2),
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = Guid.NewGuid(),
                                Quantity = 3,
                                UnitPrice = 50m
                            }
                        }
                    };
                    order2.TotalAmount = order2.Items.Sum(i => i.UnitPrice * i.Quantity);

                    dbContext.Set<Order>().AddRange(order1, order2);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
