using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class RefundOrderToGiftCertificateUsed
    {
        public RefundOrderToGiftCertificateUsed()
        {
            Messages = new List<string>();
        }

        public int IdOrder { get; set; }

        public int IdGiftCertificate { get; set; }

        public decimal AmountUsedOnSourceOrder { get; set; }

        public decimal AmountRefunded { get; set; }

        public decimal Amount { get; set; }

        public string Code { get; set; }

        public IList<string> Messages { get; set; }
    }
}