using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.GC
{
    public class GCInfoModel : BaseModel
    {
		public string Code { get; set; }

        public decimal Amount { get; set; }

        public string AmountLine { get; set; }

        public bool IsActive { get; set; }

        public GCInfoModel(GiftCertificate item)
        {
            if(item!=null)
            {
                Code = item.Code;
                Amount = item.Balance;
                AmountLine = item.Balance.ToString("C");
                IsActive = item.StatusCode == RecordStatusCode.Active;
                if(Amount==0)
                {
                    IsActive = false;
                }
            }
        }
    }
}
