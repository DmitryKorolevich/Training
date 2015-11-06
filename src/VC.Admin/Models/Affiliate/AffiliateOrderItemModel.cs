using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;

namespace VC.Admin.Models.Affiliate
{
    public class AffiliateOrderItemModel : BaseModel
    {
        public DateTime DateCreated { get; set; }

        public int IdOrder { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public int RepeatInCustomer { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal Total { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Commission { get; set; }

        public bool NewCustomerOrder { get; set; }

        public AffiliateOrderItemModel()
        {
        }
    }
}