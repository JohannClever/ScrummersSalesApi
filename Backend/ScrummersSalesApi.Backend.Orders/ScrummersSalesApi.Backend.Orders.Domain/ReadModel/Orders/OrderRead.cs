using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Orders
{
    // --- Dapper: records para lecturas ---
    public record OrderRead(
        Guid Id, Guid CustomerId, int Status, decimal TotalAmount, DateTime OrderDate,
        Guid? ItemId, Guid? ProductId, int? Quantity, decimal? UnitPrice
    );
}
