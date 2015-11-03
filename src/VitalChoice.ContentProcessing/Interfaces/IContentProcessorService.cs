using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentProcessorService
    {
        Task<IDictionary<string, object>> ExecuteAsync(string processorName, IDictionary<string, object> queryData);
        Task ExecuteAsync(string processorName, IDictionary<string, object> queryData,
            IDictionary<string, object> modelContainer);
    }
}