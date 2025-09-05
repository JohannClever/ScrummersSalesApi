using System.Data;
using System.Linq.Expressions;

namespace ScrummersSalesApi.Backend.Products.Domain.Ports
{
    public interface IGenericRepository<E> : IDisposable where E : Entities.Generic.DomainEntity
    {
        Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter = null,
                                      Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
                                      bool isTracking = false, 
                                      params Expression<Func<E, object>>[] includeObjectProperties);
        Task<E> GetByIdAsync(object id);
        Task<E> AddAsync(E entity);
        Task<IEnumerable<E>> AddAsync(IEnumerable<E> entities);
        Task UpdateAsync(E entity);
        Task DeleteAsync(E entity);
        Task DeleteAsync(IEnumerable<E> entities);
        Task<object> ExecuteStoreProcedureNonQueryAsync<T>(
            string storeProcedure,
            T entity, 
            string[] excludeProperties,
            string outPutValue = "",
            SqlDbType outPutValueType = SqlDbType.Int);
    }
}
