using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerNoteOptionValue : OptionValue<CustomerNoteOptionType>
    {
        public int IdCustomerNote { get; set; }
    }
}