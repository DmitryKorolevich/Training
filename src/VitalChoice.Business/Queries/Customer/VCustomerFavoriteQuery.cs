using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Queries.Customer
{
    public class VCustomerFavoriteQuery : QueryObject<VCustomerFavorite>
    {
		public VCustomerFavoriteQuery WithCustomerId(int? idCustomer)
		{
			if (idCustomer.HasValue)
			{
				Add(x => x.IdCustomer == idCustomer.Value);
			}
			return this;
		}
	}
}