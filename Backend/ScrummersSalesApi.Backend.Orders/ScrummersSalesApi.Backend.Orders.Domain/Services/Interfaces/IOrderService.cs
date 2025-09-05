using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;

namespace ScrummersSalesApi.Backend.Orders.Domain.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetOrdersAsync(int page, int pageSize);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<Guid> CreateOrderAsync(Order model);
        Task UpdateStatusAsync(Guid id, OrderStatus status);
    }
}
