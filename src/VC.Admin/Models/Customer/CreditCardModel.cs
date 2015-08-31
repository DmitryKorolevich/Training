using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.Validation.Logic.Interfaces;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
    public class CheckPaymentModel : BaseModel
    {
        public CheckPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.Check;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public string CheckNumber { get; set; }

        [Map]
        public bool PaidInFull { get; set; }
    }

    public class OacPaymentModel : BaseModel
    {
        public OacPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.Oac;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public int Terms { get; set; }

        [Map]
        public int Fob { get; set; }
    }

    public class CreditCardModel : BaseModel
    {
        public CreditCardModel()
        {
            PaymentMethodType = PaymentMethodType.CreditCard;
            CardType = CreditCardType.MasterCard;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public string NameOnCard { get; set; }

        [Map]
        public string CardNumber { get; set; }

        public int? ExpirationDateMonth { get; set; }

        public int? ExpirationDateYear { get; set; }

        [Map]
        public CreditCardType CardType { get; set; }

        public bool IsSelected { get; set; }
    }
}