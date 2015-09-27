using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class ProductCategoryTreeFilter : FilterBase
    {
        public IList<RecordStatusCode> Statuses { get; set; }
    }
}