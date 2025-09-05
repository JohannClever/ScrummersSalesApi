using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrummersSalesApi.Backend.Orders.Domain.Entities;
using ScrummersSalesApi.Backend.Orders.Domain.Entities.Orders;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.DataAccess.Configuration.Orders
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            builder.ToTable("Orders", "dbo");

            builder.HasKey(o => o.Id)
                   .HasName("PK_Orders");

            builder.Property(o => o.Id)
                   .HasColumnName("Id")
                   .IsRequired()
                   .ValueGeneratedNever(); // Guid generated in code

            builder.Property(o => o.CustomerId)
                   .HasColumnName("CustomerId")
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.Status)
                   .HasColumnName("Status")
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(o => o.TotalAmount)
                   .HasColumnName("TotalAmount")
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.OrderDate)
                   .HasColumnName("OrderDate")
                   .HasDefaultValueSql("SYSDATETIME()")
                   .IsRequired();

            // Relation with OrderItems
            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
