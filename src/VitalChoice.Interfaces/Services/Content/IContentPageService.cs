using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IContentPageService
    {
        Task<PagedList<ContentPage>> GetContentPagesAsync(ContentPageListFilter filter);
        Task<ContentPage> GetContentPageAsync(int id);
        Task<ContentPage> UpdateContentPageAsync(ContentPage contentPage);
        Task<bool> AttachContentPageToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteContentPageAsync(int id);
    }
}
