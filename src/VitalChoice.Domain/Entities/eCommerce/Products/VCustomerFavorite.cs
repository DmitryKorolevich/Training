namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class VCustomerFavorite: Entity
    {
	    public string ProductName { get; set; }

	    public string ProductThumbnail { get; set; }

	    public int IdCustomer { get; set; }

	    public int Quantity { get; set; }
    }
}