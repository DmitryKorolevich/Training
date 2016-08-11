using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class MasterTmsUdoInfo
    {
        public MasterTmsUdoInfo()
        {
            PRODUCTLIST=new List<MasterTmsUdoItemInfo>();
        }

        public string CID { get; set; }
        public string TYPE { get; set; }
        public string DISCOUNT { get; set; }
        public string OID { get; set; }
        public string CURRENCY { get; set; }
        public string COUPON { get; set; }
        public string FIRECJ { get; set; }
        public ICollection<MasterTmsUdoItemInfo> PRODUCTLIST { get; set; }
    }
}
