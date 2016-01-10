using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IGcService
    {
        Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter);

        Task<PagedList<GiftCertificate>> GetGiftCertificatesWithOrderInfoAsync(GCFilter filter);

        Task<GiftCertificate> GetGiftCertificateAsync(int id);

        Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model);

        Task<ICollection<GiftCertificate>> AddGiftCertificatesAsync(int quantity,GiftCertificate model);

        Task<bool> SendGiftCertificateEmailAsync(BasicEmail model);

        Task<bool> DeleteGiftCertificateAsync(int id);

	    Task<List<GiftCertificate>> GetGiftCertificatesAsync(Expression<Func<GiftCertificate, bool>> expression);

        string GenerateGCCode();
    }
}
