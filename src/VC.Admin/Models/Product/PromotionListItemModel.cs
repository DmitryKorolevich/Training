using System;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Product
{
    public class PromotionListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerType? Assigned { get; set; }

        public PromotionType PromotionType { get; set; }

        public string PromotionTypeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime DateCreated { get; set; }

        public string AddedByAgentId { get; set; }

        public PromotionListItemModel(PromotionDynamic item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Description = item.Description;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                PromotionType = (PromotionType)(item.IdObjectType ?? 0);
                PromotionTypeName = LookupHelper.GetPromotionTypeName((PromotionType)(item.IdObjectType ?? 0));
                StartDate = item.StartDate;
                ExpirationDate = item.ExpirationDate;
                DateCreated = item.DateCreated;
                if(item.DictionaryData.ContainsKey("AddedByAgentId"))
                {
                    AddedByAgentId = (string)item.DictionaryData["AddedByAgentId"];
                }
                ExpirationDate = ExpirationDate?.AddDays(-1);
            }
        }
    }
}