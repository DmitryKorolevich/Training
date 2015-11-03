﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Affiliate
{
    public class OrderPaymentsModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public ICollection<OrderPaymentLineModel> Items { get; set; }
    }
}
