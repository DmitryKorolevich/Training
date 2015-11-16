using System;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Product
{
    public class PromotionListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int StatusCode { get; set; }

        public CustomerType? Assigned { get; set; }

        public PromotionType PromotionType { get; set; }

        public string PromotionTypeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime DateCreated { get; set; }

        public string AddedByAgentId { get; set; }

        public bool Expired { get; set; }

        public PromotionListItemModel(PromotionDynamic item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Description = item.Description;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                PromotionType = (PromotionType)item.IdObjectType;
                PromotionTypeName = LookupHelper.GetPromotionTypeName((PromotionType)item.IdObjectType);
                StartDate = item.StartDate;
                ExpirationDate = item.ExpirationDate;
                DateCreated = item.DateCreated;
                if(item.DictionaryData.ContainsKey("AddedByAgentId"))
                {
                    AddedByAgentId = (string)item.DictionaryData["AddedByAgentId"];
                }
                ExpirationDate = ExpirationDate;
                if (DateTime.Now.AddDays(-1) >= ExpirationDate)
                {
                    Expired = true;
                }
            }
        }
    }
}