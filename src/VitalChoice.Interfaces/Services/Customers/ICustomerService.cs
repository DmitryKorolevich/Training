using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Interfaces.Services.Customers
{
	public interface ICustomerService : IDynamicServiceAsync<CustomerDynamic, Customer>
	{
	    Task<bool> GetCustomerCardExist(int idCustomer, int idPaymentMethod);

	    Task<bool> GetCustomerHasAffiliateOrders(int idCustomer, int? excludeIdOrder = null);

	    Task<IList<OrderNote>> GetAvailableOrderNotesAsync(CustomerType customerType);

		Task<IList<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerType? customerType);

	    Task<CustomerStatus?> GetCustomerStatusAsync(int idCustomer);

        Task<PagedList<ExtendedVCustomer>> GetCustomersAsync(CustomerFilter filter);

        Task<ICollection<CustomerOrderStatistic>> GetCustomerOrderStatistics(ICollection<int> ids);

        Task<string> UploadFileAsync(byte[] file, string fileName, Guid customerPublicId, string contentType = null);

		Task<Blob> DownloadFileAsync(string fileName, string customerPublicId);

	    Task<CustomerDynamic> InsertAsync(CustomerDynamic model, string password);

		Task<CustomerDynamic> UpdateAsync(CustomerDynamic model, string password);

        Task<ICollection<string>> GetAddressFieldValuesByValueAsync(ValuesByFieldValueFilter filter);

        Task<ICollection<string>> GetCustomerStaticFieldValuesByValue(ValuesByFieldValueFilter filter);

        Task<string> GetNewOrderNotesBasedOnCustomer(int idCustomer);

	    Task<CustomerDynamic> GetByEmailAsync(string email);

		Task ActivateGuestAsync(string email, string token, string newPassword);

	    Task UpdateEcommerceOnlyAsync(CustomerDynamic customer);

	    Task<long> GetActiveOrderCount(int idCustomer);

	    Task<bool> MergeCustomersAsync(int idCustomerPrimary, ICollection<int> customerIds);

        #region Reports 

        Task<WholesaleSummaryReport> GetWholesaleSummaryReportAsync();

	    Task<ICollection<WholesaleSummaryReportMonthStatistic>> GetWholesaleSummaryReportMonthStatisticAsync(DateTime lastMonthStartDay, int monthCount);

	    Task<PagedList<WholesaleListitem>> GetWholesalesAsync(WholesaleFilter filter);

	    #endregion
	}
}
