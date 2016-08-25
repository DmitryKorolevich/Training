using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class HeadTrackScriptsModel : BaseTrackScriptsModel
    {
        public string MasterTmsUdo { get; set; }
        public bool EnableOrderCompleteTrack { get; set; }
    }
}
