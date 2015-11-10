using System;
using System.ComponentModel.DataAnnotations.Schema;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.eCommerce.GiftCertificates
{
    public class GiftCertificate : Entity
    {
        public DateTime Created { get; set; }

        public string Code { get; set; }

        public decimal Balance { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int? UserId { get; set; }

        [NotMapped]
        public ApplicationUser User { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public GCType GCType { get; set; }

        public Guid PublicId { get; set; }

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
            toReturn.UserId = this.UserId;
            return toReturn;
        }
    }
}