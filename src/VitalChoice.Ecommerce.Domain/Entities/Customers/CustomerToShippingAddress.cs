using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerToShippingAddress : Entity
    {
        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public int IdAddress { get; set; }

        public Address ShippingAddress { get; set; }
    }
}
