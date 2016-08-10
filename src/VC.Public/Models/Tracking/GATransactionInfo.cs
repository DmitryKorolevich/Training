using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class GATransactionInfo
    {
        public int id { get; set; }
        public string affiliation { get; set; }
        public decimal revenue { get; set; }
        public decimal shipping { get; set; }
        public decimal tax { get; set; }
    }
}
