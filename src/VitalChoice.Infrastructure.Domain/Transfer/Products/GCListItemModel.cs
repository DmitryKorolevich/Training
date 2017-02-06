using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class GCListItemModel
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

        public int? IdOrder { get; set; }

        public int StatusCode { get; set; }

        public GCType GCType { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string Tag { get; set; }

        public GCListItemModel(GiftCertificate item, string productName)
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
                StatusCode = (int)item.StatusCode;
                GCType = item.GCType;
                IdOrder = item.IdOrder;
                if (!string.IsNullOrEmpty(item.FirstName) || !String.IsNullOrEmpty(item.LastName))
                {
                    RecipientName = $"{item.FirstName} {item.LastName} ";
                }
                if (!string.IsNullOrEmpty(item.Email))
                {
                    RecipientEmail = $"({item.Email})";
                }
                ProductName = productName;
                AgentId = item.AgentId;
                ExpirationDate = item.ExpirationDate;
                Tag = item.Tag;
            }
        }
    }
}