using Microsoft.EntityFrameworkCore;
using ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static class StoreProcedureExtensions
    {
        public static void CheckSpHasBeenCreated(this OrdersDbContext dbContext, string procedureName)
        {
            var connection = dbContext.Database.GetDbConnection();

            bool spExists = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT* FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = '{procedureName}' AND ROUTINE_TYPE = 'PROCEDURE'";
                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();
                spExists = (result != null);
            }

            if (!spExists)
            {
                var solutionDirectory = PathExtension.FindSolutionBaseDirectory();

                var sqlFilePath = Path.Combine(solutionDirectory, $"Syspotec.ErrorReport.Db\\Dbo\\StoreProcedures\\{procedureName}t.sql");
                var sqlContent = File.ReadAllText(sqlFilePath);
                dbContext.Database.ExecuteSqlRaw(sqlContent);
            }
        }
    }
}
