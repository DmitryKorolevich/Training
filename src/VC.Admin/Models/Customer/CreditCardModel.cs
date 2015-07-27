using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Attributes;

namespace VC.Admin.Models.Customer
{
    public class CheckPaymentModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map]
        public string CheckNumber { get; set; }

        [Map]
        public bool PaidInFull { get; set; }
    }

    public class OacPaymentModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map]
        public int? Terms { get; set; }

        [Map]
        public int? Fob { get; set; }
    }

    public class CreditCardModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map]
        public string NameOnCard { get; set; }

        [Map]
        public string CardNumber { get; set; }

        public string ExpirationDateMonth { get; set; }

        public string ExpirationDateYear { get; set; }

        [Map]
        public CreditCardType? CardType { get; set; }
    }
}