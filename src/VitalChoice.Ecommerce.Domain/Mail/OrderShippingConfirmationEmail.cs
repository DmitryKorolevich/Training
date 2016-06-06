using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class OrderShippingConfirmationEmail : EmailTemplateDataModel
    {
        public OrderShippingConfirmationEmail()
        {
            TrackingInfoItems=new List<TrackingInfoEmailItem>();
        }

        public string PublicHost { get; set; }

        [Map]
        public int? SendSide { get; set; }

        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        public bool IsPerishable { get; set; }

        public string Email { get; set; }

        public string ToEmail { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        [Map]
        public int? IdObjectType { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public OrderStatus? OrderStatus { get; set; }

        [Map]
        public OrderStatus? POrderStatus { get; set; }

        [Map]
        public OrderStatus? NPOrderStatus { get; set; }

        public AddressEmailItem BillToAddress { get; set; }

        public AddressEmailItem ShipToAddress { get; set; }

        public string Carrier { get; set; }

        public string ServiceUrl { get; set; }

        public IList<TrackingInfoEmailItem> TrackingInfoItems { get; set; }
    }
}