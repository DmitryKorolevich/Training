using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentProcessor<TResult, TModel> : IContentProcessor<TResult, TModel>
    {
        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        public abstract Task<TResult> ExecuteAsync(TModel model);

        public async Task<TResult> ExecuteAsync(IDictionary<string, object> queryData)
        {
            return await ExecuteAsync(_mapper.FromDictionary(queryData));
        }

        public async Task<object> ExecuteUntypedAsync(IDictionary<string, object> queryData)
        {
            return await ExecuteAsync(_mapper.FromDictionary(queryData));
        }

        public abstract string ResultName { get; }
    }
}