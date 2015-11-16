using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

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