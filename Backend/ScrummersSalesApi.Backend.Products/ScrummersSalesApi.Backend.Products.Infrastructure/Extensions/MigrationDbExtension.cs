using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Extensions
{
    public static class MigrationDbExtension
    {
        /// <summary>
        /// Applies .sql scripts from a folder if the database provider is SQL Server.
        /// Assumes ProductsDbContext has a method CheckTableHasBeenCreated(tableName, scriptPath).
        /// </summary>
        public static void ApplyTablesFromFolder(this WebApplication app, string solutionDirectory)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

            if (!dbContext.Database.IsSqlServer())
                return;

            // Path to the folder containing SQL scripts
            var sqlFolder = Path.Combine(
                solutionDirectory,
                "ScrummersSalesApi.Backend.Products",
                "ScrummersSalesApi.Backend.Products.Db",
                "Dbo",
                "Tables"
            );

            if (!Directory.Exists(sqlFolder))
                return;

            // Get *.sql files ordered by name
            var tables = Directory.EnumerateFiles(sqlFolder, "*.sql", SearchOption.TopDirectoryOnly)
                                  .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                                  .ToList();

            foreach (var filePath in tables)
            {
                var tableName = Path.GetFileNameWithoutExtension(filePath);
                dbContext.CheckTableHasBeenCreated(tableName, filePath);
            }
        }
    }
}
