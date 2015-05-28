using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Product;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(GCManageModelValidator))]
    public class GCManageModel : Model<GiftCertificate, IMode>
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; }

        [Localized(GeneralFieldNames.Amount)]
        public decimal Balance { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string Email { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public GCType GCType { get; set; }

        public GCManageModel()
        {
        }

        public GCManageModel(GiftCertificate item)
        {
            if (item != null)
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
            }
        }

        public override GiftCertificate Convert()
        {
            GiftCertificate toReturn = new GiftCertificate();
            toReturn.Id = Id;
            toReturn.Balance = Balance;
            toReturn.FirstName = FirstName;
            toReturn.LastName = LastName;
            toReturn.Email = Email;
            toReturn.StatusCode = StatusCode;

            return toReturn;
        }
    }
}