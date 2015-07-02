using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public interface IObjectRepositoryAsync<T, TEntity> : IReadObjectRepositoryAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<TEntity> InsertAsync(T model);
        Task<TEntity> UpdateAsync(T model);
        Task<List<TEntity>> InsertRangeAsync(ICollection<T> models);
        Task<List<TEntity>> UpdateRangeAsync(ICollection<T> models);
        Task<bool> DeleteAllAsync(ICollection<T> models);
        Task<bool> DeleteAsync(T model);
        TEntity Insert(T model);
        TEntity Update(T model);
        List<TEntity> InsertRange(ICollection<T> models);
        List<TEntity> UpdateRange(ICollection<T> models);
        bool DeleteAll(ICollection<T> models);
        bool Delete(T model);
    }
}