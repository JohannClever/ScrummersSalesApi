namespace ScrummersSalesApi.Backend.Orders.Domain.Entities.Generic
{
    public class EntityBase<T> : DomainEntity, IEntityBase<T>
    {
        public virtual T Id { get; set; }
    }
}
