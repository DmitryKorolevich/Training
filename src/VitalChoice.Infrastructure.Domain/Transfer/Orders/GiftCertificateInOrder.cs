﻿using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class GiftCertificateInOrder
    {
        public GiftCertificate GiftCertificate { get; set; }
        public decimal Amount { get; set; }
        public decimal? PAmount { get; set; }
        public decimal? NPAmount { get; set; }
    }
}