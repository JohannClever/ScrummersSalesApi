using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess
{
    public interface IDbContext<out T> where T : DbContext
    {
        T Context { get; }
    }
}
