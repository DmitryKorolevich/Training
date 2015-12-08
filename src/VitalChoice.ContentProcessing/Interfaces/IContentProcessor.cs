using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentProcessor
    {
        Task<object> ExecuteUntypedAsync(ContentViewContext viewContext);
        string ResultName { get; }
    }

    public interface IContentProcessor<TResult> : IContentProcessor
    {
        Task<TResult> ExecuteAsync(ContentViewContext viewContext);
    }
}