using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Transfer.Azure;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Customers
{
	public interface ICustomerService : IDynamicObjectServiceAsync<CustomerDynamic, Customer>
	{
		Task<IList<OrderNote>> GetAvailableOrderNotesAsync(CustomerType customerType);

		Task<IList<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerType? customerType);

		Task<PagedList<ExtendedVCustomer>> GetCustomersAsync(CustomerFilter filter);

		Task<string> UploadFileAsync(byte[] file, string fileName, string customerPublicId, string contentType = null);

		Task<Blob> DownloadFileAsync(string fileName, string customerPublicId);
	}
}
