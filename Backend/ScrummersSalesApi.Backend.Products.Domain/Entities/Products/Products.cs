using ScrummersSalesApi.Backend.Products.Domain.Entities.Generic;

namespace ScrummersSalesApi.Backend.Products.Domain.Entities.Products
{
    public class Product : EntityBase<Guid>
    {
        public Product()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
