using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Affiliates
{
    public interface IAffiliateService : IDynamicServiceAsync<AffiliateDynamic, Affiliate>
    {
        #region Affiliates

        Task<PagedList<VAffiliate>> GetAffiliatesAsync(VAffiliateFilter filter);

        Task<bool> SendAffiliateEmailAsync(BasicEmail model);

        Task<AffiliateDynamic> InsertAsync(AffiliateDynamic model, string password);

        Task<AffiliateDynamic> UpdateAsync(AffiliateDynamic model, string password);

        #endregion

        #region AffiliatePayments

        Task<PagedList<VCustomerInAffiliate>> GetCustomerInAffiliateReport(FilterBase filter);

        Task<ICollection<AffiliatePayment>> GetAffiliatePayments(int idAffiliate);

        Task<PagedList<AffiliateOrderPayment>> GetAffiliateOrderPayments(AffiliateOrderPaymentFilter filter);

        Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter);

        Task<bool> DeleteAffiliateOrderPayment(int idOrder);

        Task<AffiliateOrderPayment> UpdateAffiliateOrderPayment(AffiliateOrderPayment item);

        #endregion
    }
}
