﻿using System;
using VitalChoice.Business.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VC.Admin.Models.Product
{
    public class GCListItemModel : BaseModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; }

        public decimal Balance { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string RecipientName { get; set; }

        public string RecipientEmail { get; set; }

        public string ProductName { get; set; }

        public string AgentId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public GCType GCType { get; set; }

        public GCListItemModel(GiftCertificate item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Created = item.Created;
                Code = item.Code;
                Balance = item.Balance;
                FirstName = item.FirstName;
                LastName = item.LastName;
                Email = item.Email;
                StatusCode = item.StatusCode;
                GCType = item.GCType;
                if (!string.IsNullOrEmpty(item.FirstName) || !String.IsNullOrEmpty(item.LastName))
                {
                    RecipientName = $"{item.FirstName} {item.LastName} ";
                }
                if (!string.IsNullOrEmpty(item.Email))
                {
                    RecipientEmail = $"({item.Email})";
                }
                ProductName = LookupHelper.GetGCTypeName(item.GCType);
                AgentId = item.AgentId;
            }
        }
    }
}