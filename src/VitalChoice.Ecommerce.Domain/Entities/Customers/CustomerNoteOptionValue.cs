using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerNoteOptionValue : OptionValue<CustomerNoteOptionType>
    {
        public int IdCustomerNote { get; set; }
    }
}