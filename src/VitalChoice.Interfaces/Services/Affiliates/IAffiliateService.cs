using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;

namespace VitalChoice.Interfaces.Services.Affiliates
{
    public interface IAffiliateService : IDynamicServiceAsync<AffiliateDynamic, Affiliate>
    {
        #region Affiliates

        Task<PagedList<VAffiliate>> GetAffiliatesAsync(VAffiliateFilter filter);

        Task<bool> SendAffiliateEmailAsync(BasicEmail model);

        Task<AffiliateDynamic> InsertAsync(AffiliateDynamic model, string password);

        Task<AffiliateDynamic> UpdateAsync(AffiliateDynamic model, string password);

        Task<bool> SelectAnyAsync(int id);

        #endregion

        #region AffiliatePayments

        Task<PagedList<VCustomerInAffiliate>> GetCustomerInAffiliateReport(FilterBase filter);

        Task<ICollection<AffiliatePayment>> GetAffiliatePayments(int idAffiliate);

        Task<PagedList<AffiliateOrderPayment>> GetAffiliateOrderPayments(AffiliateOrderPaymentFilter filter);

        Task<bool> DeleteAffiliateOrderPayment(int idOrder);

        Task<AffiliateOrderPayment> UpdateAffiliateOrderPayment(AffiliateOrderPayment item);

        Task<bool> PayForAffiliateOrders(int idAffiliate, DateTime to);

        Task<AffiliatesSummaryModel> GetAffiliatesSummary();

        Task<ICollection<AffiliatesSummaryReportItemModel>> GetAffiliatesSummaryReportItemsForMonths(DateTime lastMonthStartDay, int monthCount);
        
        #endregion
    }
}
