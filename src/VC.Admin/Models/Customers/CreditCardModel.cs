using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Customer
{
    public class CheckPaymentModel : BaseModel
    {
        public CheckPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.Check;
            PaidInFull = true;
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

        public int? IdCustomerPaymentMethod { get; set; }
    }

    public class WireTransferPaymentModel : BaseModel
    {
        public WireTransferPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.WireTransfer;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public string PaymentComment { get; set; }
    }

    public class MarketingPaymentModel : BaseModel
    {
        public MarketingPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.Marketing;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public string PaymentComment { get; set; }

        [Map]
        public int? MarketingPromotionType { get; set; }
    }

    public class VCWellnessEmployeeProgramPaymentModel : BaseModel
    {
        public VCWellnessEmployeeProgramPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.VCWellnessEmployeeProgram;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }

        [Map]
        public string PaymentComment { get; set; }
    }
}