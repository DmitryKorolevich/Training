using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IFAQService
    {
        Task<PagedList<FAQ>> GetFAQsAsync(FAQListFilter filter);
        Task<FAQ> GetFAQAsync(int id);
        Task<FAQ> UpdateFAQAsync(FAQ faq);
        Task<bool> AttachFAQToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteFAQAsync(int id);
    }
}
