using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentProcessor
    {
        Task<object> ExecuteUntypedAsync(IDictionary<string, object> queryData);
        string ResultName { get; }
    }

    public interface IContentProcessor<TResult> : IContentProcessor
    {
        Task<TResult> ExecuteAsync(IDictionary<string, object> queryData);
    }

    public interface IContentProcessor<TResult, in TModel>: IContentProcessor<TResult>
    {
        Task<TResult> ExecuteAsync(TModel model);
    }
}