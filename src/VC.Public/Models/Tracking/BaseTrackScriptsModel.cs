using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.Models.Tracking
{
    public class BaseTrackScriptsModel
    {
        public bool OrderCompleteStep { get; set; }
        public OrderDynamic Order { get; set; }
        public bool MyBuysEnabled { get; set; }
    }
}
