using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class AffiliateListItemModel
    {
        public int Id { get; set; }

        public AffiliateStatus StatusCode { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public string WebSite { get; set; }

        public decimal CommissionFirst { get; set; }

        public decimal CommissionAll { get; set; }

        public string CommissionDescription { get; set; }

        public string Tier { get; set; }

        public int CustomersCount { get; set; }

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        public decimal NotPaidCommissionsAmount { get; set; }

        public long? NotPaidCommissionsCount { get; set; }

        public int? PaymentType { get; set; }

        public AffiliateListItemModel(VAffiliate item)
        {
            if(item!=null)
            {
                Id = item.Id;
                StatusCode = item.StatusCode;
                Name = item.Name;
                Email = item.Email;
                Company = item.Company;
                WebSite = item.WebSite;
                CommissionFirst = item.CommissionFirst;
                CommissionAll = item.CommissionAll;
                CommissionDescription = $"{item.CommissionFirst:N2}% / {item.CommissionAll:N2}%";
                Tier = item.Tier;
                CustomersCount = item.CustomersCount;
                DateEdited = item.DateEdited;
                EditedByAgentId = item.EditedByAgentId;
                PaymentType = item.PaymentType;
                if (item.NotPaidCommission!=null)
                {
                    NotPaidCommissionsAmount = item.NotPaidCommission.Amount;
                    NotPaidCommissionsCount = item.NotPaidCommission.Count;
                }
            }
        }
    }
}