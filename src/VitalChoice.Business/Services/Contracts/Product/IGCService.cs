using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Business.Services.Contracts.Product
{
	public interface IGCService
    {
        Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter);
        
        Task<GiftCertificate> GetGiftCertificateAsync(int id);

        Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model);

        Task<ICollection<GiftCertificate>> AddGiftCertificatesAsync(int quantity,GiftCertificate model);

        Task<bool> SendGiftCertificateEmailAsync(GiftCertificateEmail model);

        Task<bool> DeleteGiftCertificateAsync(int id);
    }
}
