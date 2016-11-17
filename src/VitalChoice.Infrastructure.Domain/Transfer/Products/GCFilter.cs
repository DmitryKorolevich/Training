﻿using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class GCFilter : FilterBase
    {
        public string Code { get; set; }

        public string ExactCode { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public GCType? Type { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public RecordStatusCode? StatusCode { get; set; }

        public CustomerAddressFilter ShippingAddress { get; set; }

        public CustomerAddressFilter BillingAddress { get; set; }

        public DateTime? ExpirationFrom { get; set; }

        public DateTime? ExpirationTo { get; set; }

        public string Tag { get; set; }

        public bool NotZeroBalance { get; set; }
    }
}