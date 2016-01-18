using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services
{
	public interface IEmailTemplateService
    {
        Task<PagedList<EmailTemplate>> GetEmailTemplatesAsync(FilterBase filter);
        Task<EmailTemplate> GetEmailTemplateAsync(int id);
        Task<EmailTemplate> UpdateEmailTemplateAsync(EmailTemplate recipe);
        Task<bool> DeleteEmailTemplateAsync(int id);
    }
}
