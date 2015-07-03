using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public interface IObjectRepositoryAsync<T, TEntity> : IReadObjectRepositoryAsync<T, TEntity>
        where TEntity : Entity
    {
        Task<T> InsertAsync(T model);
        Task<T> UpdateAsync(T model);
        Task<List<T>> InsertRangeAsync(ICollection<T> models);
        Task<List<T>> UpdateRangeAsync(ICollection<T> models);
        Task<bool> DeleteAllAsync(ICollection<T> models);
        Task<bool> DeleteAsync(T model);
        Task<bool> DeleteAllAsync(ICollection<int> list);
        Task<bool> DeleteAsync(int id);
        T Insert(T model);
        T Update(T model);
        List<T> InsertRange(ICollection<T> models);
        List<T> UpdateRange(ICollection<T> models);
        bool DeleteAll(ICollection<T> models);
        bool Delete(T model);
        bool DeleteAll(ICollection<int> list);
        bool Delete(int id);
    }
}