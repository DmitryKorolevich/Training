using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderImportItemOrderDynamic
    {
        public OrderDynamic Order { get; set; }

        public OrderBaseImportItem OrderImportItem { get; set; }
    }
}
