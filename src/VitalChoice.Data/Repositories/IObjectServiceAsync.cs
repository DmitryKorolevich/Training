using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public interface IObjectServiceAsync<T>
    {
        Task<T> InsertAsync(T model);
        Task<T> UpdateAsync(T model);
        Task<List<T>> InsertRangeAsync(ICollection<T> models);
        Task<List<T>> UpdateRangeAsync(ICollection<T> models);
        Task<bool> DeleteAllAsync(ICollection<T> models, bool physically = false);
        Task<bool> DeleteAsync(T model, bool physically = false);
        Task<bool> DeleteAllAsync(ICollection<int> list, bool physically = false);
        Task<bool> DeleteAsync(int id, bool physically = false);
        T Insert(T model);
        T Update(T model);
        List<T> InsertRange(ICollection<T> models);
        List<T> UpdateRange(ICollection<T> models);
        bool DeleteAll(ICollection<T> models, bool physically = false);
        bool Delete(T model, bool physically = false);
        bool DeleteAll(ICollection<int> list, bool physically = false);
        bool Delete(int id, bool physically = false);
    }
}