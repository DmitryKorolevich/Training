using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class ProductCategoryProcessor : IContentProcessor
    {
        private readonly IReadRepositoryAsync<ProductCategoryContent> _productCategoryContenRepositoryAsync;

        public ProductCategoryProcessor(IReadRepositoryAsync<ProductCategoryContent> productCategoryContenRepositoryAsync)
        {
			_productCategoryContenRepositoryAsync = productCategoryContenRepositoryAsync;
        }

        public Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
			//todo: refactor useless code in service ProductViewService (or even remove that service and make one generic instead of having tons of similar code) and place model population logic here
			return Task.FromResult(model);
        }
    }
}