using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderBaseImportItem
    {
        public int RowNumber { get; set; }

        public ICollection<MessageInfo> ErrorMessages { get; set; }

        public virtual void SetFields(OrderDynamic order, CustomerPaymentMethodDynamic paymentMethod)
        {
            order.DateCreated = DateTime.Now;
            order.StatusCode = (int)RecordStatusCode.Active;
        }
    }
}
