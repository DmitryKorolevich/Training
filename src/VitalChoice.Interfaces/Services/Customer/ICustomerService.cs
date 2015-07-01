using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;

namespace VitalChoice.Interfaces.Services.Customer
{
	public interface ICustomerService
	{
		Task<IList<OrderNote>> GetAvailableOrderNotesAsync(CustomerType customerType);

		Task<IList<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerType customerType);

		Task<CustomerDynamic> AddUpdateCustomerAsync(CustomerDynamic customerDynamic);

		Task<CustomerDynamic> GetCustomerAsync(int id, bool withDefaults = false);

		Task<bool> DeleteCustomerAsync(int id);

		Task<PagedList<CustomerDynamic>> GetCustomersAsync(CustomerFilter filter);
	}
}
