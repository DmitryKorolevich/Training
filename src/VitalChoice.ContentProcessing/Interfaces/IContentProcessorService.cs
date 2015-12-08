using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentProcessorService
    {
        Task<IDictionary<string, object>> ExecuteAsync(string processorName, ContentViewContext viewContext);
        Task ExecuteAsync(string processorName, ContentViewContext viewContext,
            IDictionary<string, object> modelContainer);
    }
}