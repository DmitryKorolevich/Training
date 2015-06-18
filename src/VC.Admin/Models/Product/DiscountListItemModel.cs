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
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;

namespace VC.Admin.Models.Product
{
    public class DiscountListItemModel : Model<DiscountDynamic, IMode>
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
                AssignedName = StatusEnumHelper.GetAssignedCustomerTypeName(item.Assigned);
                DiscountType = item.DiscountType;
                DiscountTypeName = StatusEnumHelper.GetDiscountTypeName(item.DiscountType);
                StartDate = item.StartDate;
                ExpirationDate = item.ExpirationDate;
                DateCreated = item.DateCreated;
                if (item.AddedBy != null && item.AddedBy.AdminProfile != null)
                {
                    AddedByAgentId = item.AddedBy.AdminProfile.AgentId;
                }
            }
        }
    }
}