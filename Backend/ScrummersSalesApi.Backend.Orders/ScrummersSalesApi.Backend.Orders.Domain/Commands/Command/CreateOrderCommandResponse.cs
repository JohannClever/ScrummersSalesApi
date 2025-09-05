using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commands.Command
{
    public class CreateOrderCommandResponse
    {
        public Guid ProductId { get; set; }
    }
}
