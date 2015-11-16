using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerNoteOptionTypeQuery : OptionTypeQuery<CustomerNoteOptionType>
    {
        public override IQueryOptionType<CustomerNoteOptionType> WithObjectType(int? objectType)
        {
            return this;
        }
    }
}