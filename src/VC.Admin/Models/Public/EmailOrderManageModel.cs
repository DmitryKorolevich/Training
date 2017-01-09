using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VC.Admin.Validators.Order;
using VC.Admin.Validators.Public;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Public
{
    [ApiValidator(typeof(EmailOrderManageModelValidator))]
    public class EmailOrderManageModel : BaseModel
    {
        public AddressModel Shipping { get; set; }

        public MarketingPaymentModel Marketing { get; set; }

        public NCPaymentModel NC { get; set; }

        public int? IdPaymentMethodType { get; set; }

        public string DetailsOnEvent { get; set; }

        public string Instuction { get; set; }

        public int? IdRequestor { get; set; }

        public int? IdReason { get; set; }

        public IList<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public EmailOrderManageModel()
        {
            SkuOrdereds = new List<SkuOrderedManageModel>();
        }

    }
}