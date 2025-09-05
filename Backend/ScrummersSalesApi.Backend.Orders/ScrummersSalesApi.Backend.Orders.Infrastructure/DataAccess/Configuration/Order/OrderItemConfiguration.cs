using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrummersSalesApi.Backend.Orders.Domain.Entities;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess.Configuration.Orders
{
    /// <summary>
    /// EF Core configuration for OrderItem entity (dbo.OrderItems).
    /// </summary>
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            // Map to table and schema
            builder.ToTable("OrderItems", "dbo");

            // Primary key (Guid generated in code)
            builder.HasKey(i => i.Id)
                   .HasName("PK_OrderItems");

            builder.Property(i => i.Id)
                   .HasColumnName("Id")
                   .IsRequired()
                   .ValueGeneratedNever();

            // Foreign keys / columns
            builder.Property(i => i.OrderId)
                   .HasColumnName("OrderId")
                   .IsRequired();

            builder.Property(i => i.ProductId)
                   .HasColumnName("ProductId")
                   .IsRequired();

            builder.Property(i => i.Quantity)
                   .HasColumnName("Quantity")
                   .IsRequired();

            builder.Property(i => i.UnitPrice)
                   .HasColumnName("UnitPrice")
                   .HasPrecision(18, 2)
                   .IsRequired();

            // Relationship: many items per order; FK defined here.
            // Deletion is configured on the principal (OrderConfiguration) as Cascade.
            builder.HasOne<Order>()
                   .WithMany(o => o.Items)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_OrderItems_Orders");

            // Useful indexes for lookups/filtering
            builder.HasIndex(i => i.OrderId).HasDatabaseName("IX_OrderItems_OrderId");
            builder.HasIndex(i => i.ProductId).HasDatabaseName("IX_OrderItems_ProductId");

            // Check constraints (SQL Server syntax)
            builder.HasCheckConstraint("CK_OrderItems_Quantity_Positive", "[Quantity] > 0");
            builder.HasCheckConstraint("CK_OrderItems_UnitPrice_Positive", "[UnitPrice] > 0");
        }
    }
}
