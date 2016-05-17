using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class OrderProductReviewEmailProductItem
    {
        public string Thumbnail { get; set; }

        public string DisplayName { get; set; }

        public string ProductUrl { get; set; }
    }

    public class OrderProductReviewEmail : EmailTemplateDataModel
    {
        public OrderProductReviewEmail()
        {
            Products=new List<OrderProductReviewEmailProductItem>();
        }

        public string Email { get; set; }

        public string PublicHost { get; set; }

        public string CustomerName { get; set; }

        public string UrlEncodedEmail { get; set; }

        public IList<OrderProductReviewEmailProductItem> Products { get; set; }
    }
}