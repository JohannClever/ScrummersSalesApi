using Microsoft.EntityFrameworkCore;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
{
    public static class CreateTablesExtension
    {
        public static void CheckTableHasBeenCreated(this DataAccess.ProductsDbContext dbContext, string tableName ,string tableRootPath)
        {
            var connection = dbContext.Database.GetDbConnection();

            bool tableExists = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();
                tableExists = (result != null);
            }


            if (!tableExists)
            {
                var solutionDirectory = PathExtension.FindSolutionBaseDirectory();
                var sqlContent = File.ReadAllText(tableRootPath);
                dbContext.Database.ExecuteSqlRaw(sqlContent);
            }
        }
    }
}

