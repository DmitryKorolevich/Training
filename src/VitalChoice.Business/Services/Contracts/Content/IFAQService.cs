using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Business.Services.Contracts.Content
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
