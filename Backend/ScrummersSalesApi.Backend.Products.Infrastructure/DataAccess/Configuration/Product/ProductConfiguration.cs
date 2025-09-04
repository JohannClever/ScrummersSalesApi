using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess.Configuration.Products
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            // Usa schema dbo y nombre con mayúscula (igual que la tabla real)
            builder.ToTable("Products", "dbo");

            builder.HasKey(p => p.Id)
                   .HasName("PK_Products");

            builder.Property(p => p.Id)
                   .HasColumnName("Id")
                   .IsRequired()
                   // Como tú generas el Guid en el constructor, NO debe ser generado por la BD
                   .ValueGeneratedNever();

            builder.Property(p => p.Name)
                   .HasColumnName("Name")
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasColumnName("Description");

            builder.Property(p => p.Price)
                   .HasColumnName("Price")
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(p => p.Stock)
                   .HasColumnName("Stock")
                   .IsRequired();

            builder.Property(p => p.CreateDate)
                   .HasColumnName("CreateDate")
                   .IsRequired()
                   // SQL Server: usa SYSDATETIME() (no "now()")
                   .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(p => p.UpdatedDate)
                   .HasColumnName("UpdateDate")
                   .IsRequired()
                   .HasDefaultValueSql("SYSDATETIME()");

            // Nombres y sintaxis de SQL Server en los check constraints:
            builder.HasCheckConstraint("CK_Products_Price_Positive", "[Price] > 0");
            builder.HasCheckConstraint("CK_Products_Stock_NonNegative", "[Stock] >= 0");
        }
    }
}
