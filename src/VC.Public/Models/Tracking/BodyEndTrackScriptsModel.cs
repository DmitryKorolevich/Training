using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.Models.Tracking
{
    public class BodyEndTrackScriptsModel : BaseTrackScriptsModel
    {
        public string BrontoOrderInfo { get; set; }
        public string GoogleActionCheckout { get; set; }
        public string GoogleActionPurchase { get; set; }
        public string CustomerEmail { get; set; }
        public string Criteo { get; set; }
        public string PepperjamQuery { get; set; }
    }
}
