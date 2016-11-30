using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.Models.Tracking
{
    public enum CheckoutStep
    {
        Unknown = 0,
        ViewCart,
        Welcome,
        Billing,
        Shipping,
        Preview,
        Receipt
    }

    public class BaseTrackScriptsModel
    {
        public CheckoutStep Step { get; set; }
        public bool OrderCompleteStep { get; set; }
        public OrderDynamic Order { get; set; }
        public bool MyBuysEnabled { get; set; }
    }
}
