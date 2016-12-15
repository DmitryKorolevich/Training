﻿using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Discounts
{
    public class DiscountListItemModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int StatusCode { get; set; }

        public CustomerType? Assigned { get; set; }

        public DiscountType DiscountType { get; set; }

        public string DiscountTypeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime DateCreated { get; set; }

        public string AddedByAgentId { get; set; }

        public DateTime DateEdited { get; set; }

        public string EditedByAgentId { get; set; }

        public DateStatus DateStatus { get; set; }

        public DiscountListItemModel(DiscountDynamic item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Code = item.Code;
                Description = item.Description;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                DiscountType = (DiscountType)item.IdObjectType;
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
                if (now.AddDays(-1)>=ExpirationDate)
                {
                    DateStatus = DateStatus.Expired;
                }
                else
                {
                    if(now>=StartDate)
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