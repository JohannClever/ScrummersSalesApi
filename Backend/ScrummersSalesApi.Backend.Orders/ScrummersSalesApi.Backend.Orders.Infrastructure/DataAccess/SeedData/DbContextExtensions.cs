using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess.SeedData
{
    public static class DbContextExtensions
    {
        public static bool TableExists(this OrdersDbContext dbContext, string tableName)
        {
            var connection = dbContext.Database.GetDbConnection();
            var databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
            var exists = databaseCreator.Exists();

            if (exists)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
                    connection.Open();
                    var result = command.ExecuteScalar();
                    connection.Close();
                    return (result != null);
                }
            }

            return false;
        }
    }
}
