using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

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