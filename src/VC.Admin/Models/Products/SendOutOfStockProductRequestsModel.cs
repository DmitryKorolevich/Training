using System.Collections.Generic;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
{
    public class SendOutOfStockProductRequestsModel : BaseModel
    {
        public ICollection<int> Ids { get; set; }

        public string MessageFormat { get; set; }
    }
}