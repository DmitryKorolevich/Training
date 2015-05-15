using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.Product
{
    public class GiftCertificate : Entity
    {
        public DateTime Created { get; set; }

        public string Code { get; set; }

        public decimal Balance { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public GCType GCType { get; set; }

        public GiftCertificate Clone()
        {
            var toReturn = new GiftCertificate();
            toReturn.Id = this.Id;
            toReturn.Created = this.Created;
            toReturn.Code = this.Code;
            toReturn.Balance = this.Balance;
            toReturn.FirstName = this.FirstName;
            toReturn.LastName = this.LastName;
            toReturn.Email = this.Email;
            toReturn.StatusCode = this.StatusCode;
            toReturn.GCType = this.GCType;
            return toReturn;
        }
    }
}