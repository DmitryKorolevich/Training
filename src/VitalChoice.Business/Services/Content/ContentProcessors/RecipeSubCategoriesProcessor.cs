using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeSubCategoriesProcessor : ContentProcessor<List<ContentCategory>, RecipeParameters>
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentCategoryRepositoryAsync;

        public RecipeSubCategoriesProcessor(IObjectMapper<RecipeParameters> mapper,
            IReadRepositoryAsync<ContentCategory> contentCategoryRepositoryAsync) : base(mapper)
        {
            _contentCategoryRepositoryAsync = contentCategoryRepositoryAsync;
        }

        protected override Task<List<ContentCategory>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            return
                _contentCategoryRepositoryAsync.Query(
                    p => p.ParentId == viewContext.Parameters.IdCategory && p.StatusCode == RecordStatusCode.Active)
                    .OrderBy(query => query.OrderBy(p => p.Order))
                    .SelectAsync(false);
        }

        public override string ResultName => "Categories";
    }
}