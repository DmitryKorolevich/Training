using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IGcService
    {
        Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter);

        Task<GCStatisticModel> GetGiftCertificatesWithOrderInfoAsync(GCFilter filter);

        Task<GiftCertificate> GetGiftCertificateAsync(int id);

        Task<GiftCertificate> GetGiftCertificateAsync(string code);

        Task<List<GiftCertificate>> TryGetGiftCertificatesAsync(ICollection<string> codes);

        Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model);

        Task<ICollection<GiftCertificate>> AddManualGiftCertificatesAsync(int quantity,GiftCertificate model);
        
        Task<bool> DeleteGiftCertificateAsync(int id);

	    Task<List<GiftCertificate>> GetGiftCertificatesAsync(Expression<Func<GiftCertificate, bool>> expression);

        Task<string> GenerateGCCode();

        Task<ICollection<GiftCertificate>> ImportGCsAsync(byte[] file, int idAddedBy);
    }
}
