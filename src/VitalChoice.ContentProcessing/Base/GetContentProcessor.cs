using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.ContentProcessing.Base
{
    public class GetContentProcessor<TResult, TModel> : ContentProcessor<TResult, TModel>
        where TResult : ContentDataItem
        where TModel: ProcessorModel
    {
        protected readonly IRepositoryAsync<TResult> ContentRepository;

        public GetContentProcessor(IObjectMapper<TModel> mapper, IRepositoryAsync<TResult> contentRepository) : base(mapper)
        {
            ContentRepository = contentRepository;
        }

        protected virtual Expression<Func<TResult, bool>> FilterExpression(TModel model) =>
            p => p.Url == model.Url && p.StatusCode != RecordStatusCode.Deleted;

        protected virtual IQueryFluent<TResult> BuildQuery(IQueryFluent<TResult> query)
        {
            return query.Include(p => p.MasterContentItem)
                .ThenInclude(p => p.MasterContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor)
                .Include(p => p.ContentItem)
                .ThenInclude(p => p.ContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor);
        }

        public override async Task<TResult> ExecuteAsync(TModel model)
        {
            if (!string.IsNullOrEmpty(model.Url))
            {
                return
                    (await BuildQuery(ContentRepository.Query(FilterExpression(model))).SelectAsync(false))
                        .FirstOrDefault();
            }
            return null;
        }

        public override string ResultName => "Model";
    }
}