using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;

namespace VC.Admin.Models.Affiliate
{
    public class AffiliateListItemModel : BaseModel
    {
        public int Id { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string WebSite { get; set; }

        public decimal CommissionFirst { get; set; }

        public decimal CommissionAll { get; set; }

        public string Tier { get; set; }

        public int CustomersCount { get; set; }

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        public decimal? NotPaidCommissionsAmount { get; set; }

        public long? NotPaidCommissionsCount { get; set; }

        public AffiliateListItemModel(VAffiliate item)
        {
            if(item!=null)
            {
                Id = item.Id;
                StatusCode = item.StatusCode;
                Name = item.Name;
                Company = item.Company;
                WebSite = item.WebSite;
                CommissionFirst = item.CommissionFirst;
                CommissionAll = item.CommissionAll;
                Tier = item.Tier;
                CustomersCount = item.CustomersCount;
                DateEdited = item.DateEdited;
                EditedByAgentId = item.EditedByAgentId;
                if(item.NotPaidCommission!=null)
                {
                    NotPaidCommissionsAmount = item.NotPaidCommission.Amount;
                    NotPaidCommissionsCount = item.NotPaidCommission.Count;
                }
            }
        }
    }
}