using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates
{
    public class GCWithOrderListItemModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; }

        public decimal Balance { get; set; }

        public string BillingLastName { get; set; }

        public string ShippingLastName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string StatusCodeName { get; set; }

        public GCType GCType { get; set; }

        public string GCTypeName { get; set; }

        public GCWithOrderListItemModel(GiftCertificate item, ICollection<AddressOptionType> types)
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

                var lastNameOptionTypeIds = types.Where(p => p.Name == "LastName").Select(p=>p.Id).ToList();
                var address = item?.Order?.ShippingAddress;
                if(address!=null)
                {
                    ShippingLastName = address.OptionValues.FirstOrDefault(p => lastNameOptionTypeIds.Contains(p.IdOptionType))?.Value;
                }
                address = item?.Order?.PaymentMethod?.BillingAddress;
                if (address != null)
                {
                    BillingLastName = address.OptionValues.FirstOrDefault(p => lastNameOptionTypeIds.Contains(p.IdOptionType))?.Value;
                }
            }
        }
    }
}