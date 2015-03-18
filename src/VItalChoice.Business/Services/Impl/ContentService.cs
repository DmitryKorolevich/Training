using System;
using System.Linq;
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
using Microsoft.Data.Entity;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Business.Services.Impl
{
	public class ContentService : IContentService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentItemToContentItemProcessor> rRepository;
        private readonly IRepositoryAsync<Test> tRepository;

        public ContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,IRepositoryAsync<ContentItemToContentItemProcessor> rRepository,
            IRepositoryAsync<Test> tRepository)
		{
            this.masterContentItemRepository = masterContentItemRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.rRepository = rRepository;
            this.tRepository = tRepository;
        }

        public async Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, int? categoryid = null)
        {
            //var master = (masterContentItemRepository.Query(p => p.Id == 1).Select()).FirstOrDefault();
            //var contentItemToRep = (rRepository.Query(p => p.ContentItemId == 2).Select()).FirstOrDefault();
            var contentItem = (contentItemRepository.Query(p => p.Id == 2).Include(p=>p.ContentItemToContentItemProcessors.Select(pp=>pp.ContentItemProcessor)).Select()).FirstOrDefault();
            //var category = (await contentCategoryRepository.Query(p => p.Id == 2).Include(p=>p.MasterContentItem).SelectAsync()).FirstOrDefault();

            VitalChoiceContext context = new VitalChoiceContext();
            var item =  context.Set<ContentItem>().Where(p => p.Id == 2).Include(p => p.ContentItemToContentItemProcessors).ThenInclude(p => p.ContentItemProcessor).FirstOrDefault();

            //var test= tRepository.Query(p => p.Id == 1).Include(p => p.Text2s).Select().FirstOrDefault();

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
