using System;
using System.Collections.Generic;

namespace VC.Admin.Models.Orders
{
    public class OrderExportResults
    {
        public DateTime LoadTimestamp { get; set; }

        public ICollection<OrderExportRequestModel> ExportModels { get; set; }
    }
}