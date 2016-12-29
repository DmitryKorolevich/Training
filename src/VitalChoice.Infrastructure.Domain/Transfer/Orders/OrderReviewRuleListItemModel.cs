using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderReviewRuleListItemModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ApplyType ApplyType { get; set; }

        public int StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public string AddedByAgentId { get; set; }

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        public OrderReviewRuleListItemModel(OrderReviewRuleDynamic item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                ApplyType = item.ApplyType;
                StatusCode = item.StatusCode;
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
            }
        }
    }
}