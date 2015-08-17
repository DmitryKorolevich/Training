﻿using System;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Product
{
    public class DiscountListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public string AssignedName { get; set; }

        public DiscountType DiscountType { get; set; }

        public string DiscountTypeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime DateCreated { get; set; }

        public string AddedByAgentId { get; set; }

        public DiscountListItemModel(DiscountDynamic item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Code = item.Code;
                Description = item.Description;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                AssignedName = LookupHelper.GetAssignedCustomerTypeName(item.Assigned);
                DiscountType = (DiscountType)(item.IdObjectType ?? 0);
                DiscountTypeName = LookupHelper.GetDiscountTypeName((DiscountType)(item.IdObjectType ?? 0));
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