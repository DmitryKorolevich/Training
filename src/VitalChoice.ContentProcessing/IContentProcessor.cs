using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.ContentProcessing
{
    public interface IContentProcessor<TResult>
    {
        Task<TResult> ExecuteAsync(IDictionary<string, object> queryData);
    }

    public interface IContentProcessorService
    {
        Task<TResult> ExecuteAsync<TResult>(string processorName, IDictionary<string, object> queryData);
    }

    public interface IContentService<TEntity>
    {
        Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData);
    }
}
