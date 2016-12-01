using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.Models.Tracking
{
    public enum PageName
    {
        Unknown = 0,
        ProfileBilling,
        ViewCart,
        Welcome,
        Billing,
        Shipping,
        Preview,
        Receipt
    }

    public class BaseTrackScriptsModel
    {
        public PageName PageName { get; set; }
        public bool OrderCompleteStep { get; set; }
        public OrderDynamic Order { get; set; }
        public bool MyBuysEnabled { get; set; }
    }
}
