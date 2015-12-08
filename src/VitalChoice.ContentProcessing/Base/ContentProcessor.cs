using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentProcessor<TResult, TModel> : IContentProcessor<TResult>
    {
        protected class ProcessorViewContext
        {
            public ProcessorViewContext(TModel parameters, ContentDataItem entity)
            {
                Parameters = parameters;
                Entity = entity;
            }

            public TModel Parameters { get; }
            public ContentDataItem Entity { get; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return
                ExecuteAsync(new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters, false), viewContext.BaseEntity));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters, false),
                        viewContext.BaseEntity));
        }

        public abstract string ResultName { get; }
    }

    public abstract class ContentProcessor<TResult, TModel, TEntity> : IContentProcessor<TResult>
        where TEntity : ContentDataItem
    {
        protected class ProcessorViewContext
        {
            public ProcessorViewContext(TModel parameters, TEntity entity)
            {
                Parameters = parameters;
                Entity = entity;
            }

            public TModel Parameters { get; }
            public TEntity Entity { get; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return
                ExecuteAsync(new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters, false),
                    (TEntity) viewContext.BaseEntity));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters, false),
                        (TEntity) viewContext.BaseEntity));
        }

        public abstract string ResultName { get; }
    }
}