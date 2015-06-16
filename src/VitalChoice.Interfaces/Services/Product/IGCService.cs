using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Product
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
