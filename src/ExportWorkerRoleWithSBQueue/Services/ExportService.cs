using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public class OrderExportService : IOrderExportService
    {
        public void UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors)
        {
            throw new NotImplementedException();
        }
    }
}
