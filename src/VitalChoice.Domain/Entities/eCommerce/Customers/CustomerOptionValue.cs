using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerOptionValue : OptionValue<CustomerOptionType>
    {
        public int IdCustomer { get; set; }
    }
}