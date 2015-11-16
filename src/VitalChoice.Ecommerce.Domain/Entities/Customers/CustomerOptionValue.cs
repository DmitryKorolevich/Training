using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerOptionValue : OptionValue<CustomerOptionType>
    {
        public int IdCustomer { get; set; }
    }
}