using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IGcService
    {
        Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter);
        
        Task<GiftCertificate> GetGiftCertificateAsync(int id);

        Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model);

        Task<ICollection<GiftCertificate>> AddGiftCertificatesAsync(int quantity,GiftCertificate model);

        Task<bool> SendGiftCertificateEmailAsync(BasicEmail model);

        Task<bool> DeleteGiftCertificateAsync(int id);

	    Task<List<GiftCertificate>> GetGiftCertificatesAsync(Expression<Func<GiftCertificate, bool>> expression);
    }
}
