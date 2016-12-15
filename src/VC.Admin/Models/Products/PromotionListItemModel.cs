using System;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
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

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        public DateStatus DateStatus { get; set; }

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
                DateEdited = item.DateEdited;
                if (item.DictionaryData.ContainsKey("EditedByAgentId"))
                {
                    EditedByAgentId = (string)item.DictionaryData["EditedByAgentId"];
                }
                ExpirationDate = ExpirationDate;
                DateTime now = DateTime.Now;
                if (now.AddDays(-1) >= ExpirationDate)
                {
                    DateStatus = DateStatus.Expired;
                }
                else
                {
                    if (now >= StartDate)
                    {
                        DateStatus = DateStatus.Live;
                    }
                    else
                    {
                        DateStatus = DateStatus.Future;
                    }
                }
            }
        }
    }
}