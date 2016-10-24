using System;
using System.Collections.Generic;
using VC.Admin.Validators.Order;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.Orders
{
    public class ReshipProblemSkuModel
    {
        public int IdSku { get; set; }

        public string Code { get; set; }

        public bool Used { get; set; }
    }

    [ApiValidator(typeof(OrderReshipManageModelValidator))]
    public class OrderReshipManageModel : OrderManageModel
    {
        [Map]
        public int? ServiceCode { get; set; }

        [Map]
        public bool ReturnAssociated { get; set; }
        
        public decimal OrderSourceTotal { get; set; }

        public DateTime OrderSourceDateCreated { get; set; }

        public ICollection<ReshipProblemSkuModel> ReshipProblemSkus { get; set; }

        public OrderReshipManageModel()
        {
            ReshipProblemSkus = new List<ReshipProblemSkuModel>();
        }
    }
}