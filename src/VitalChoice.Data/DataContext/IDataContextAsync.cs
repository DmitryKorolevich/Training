using System.Threading;
using System.Threading.Tasks;

namespace VitalChoice.Data.DataContext
{
    public interface IDataContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}