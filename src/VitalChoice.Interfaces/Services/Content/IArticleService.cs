using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IArticleService
    {
        Task<PagedList<Article>> GetArticlesAsync(ArticleItemListFilter filter);
        Task<Article> GetArticleAsync(int id);
        Task<Article> UpdateArticleAsync(Article recipe);
        Task<bool> AttachArticleToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteArticleAsync(int id);
    }
}
