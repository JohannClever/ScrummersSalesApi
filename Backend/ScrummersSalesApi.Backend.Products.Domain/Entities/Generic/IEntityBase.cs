namespace ScrummersSalesApi.Backend.Products.Domain.Entities.Generic
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}
