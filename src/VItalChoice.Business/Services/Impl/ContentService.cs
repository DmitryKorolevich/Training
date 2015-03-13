using System;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Impl
{
	public class ContentService : IContentService
    {
		public ContentService()
		{
		}

        public async Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, int? categoryid = null)
        {
            ExecutedContentItem toReturn = new ExecutedContentItem()
            {
                HTML = "<div>Test HTML</div>",
                Title = "Test title",
                MetaDescription = "Test MetaDescription",
                MetaKeywords = "Test MetaKeywords",
            };

            await Task.Run(() => { });

            return toReturn;
        }
    }
}
