using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess
{
    public interface IDbContext<out T> where T : DbContext
    {
        T Context { get; }
    }
}
