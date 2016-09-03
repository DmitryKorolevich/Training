using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryTreeFilter : FilterBase
    {
        public ICollection<RecordStatusCode> Statuses { get; set; }
    }
}