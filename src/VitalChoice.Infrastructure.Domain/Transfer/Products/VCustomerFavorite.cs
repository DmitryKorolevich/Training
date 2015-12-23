using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class VCustomerFavorite: Entity
    {
	    public string ProductName { get; set; }

        public string ProductSubTitle { get; set; }

        public string ProductThumbnail { get; set; }

	    public int IdCustomer { get; set; }

	    public int Quantity { get; set; }
    }
}