using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Cart;
using VC.Public.Models.Profile;
using VC.Public.Validators.Checkout;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    [ApiValidator(typeof(MultipleOrdersReviewModelValidator))]
    public class MultipleOrdersReviewModel : BaseModel
    {
        public bool AutoShip { get; set; }

        public string DiscountCode { get; set; }

        public IList<CartGcModel> GiftCertificateCodes { get; set; }

        public IList<KeyValuePair<string, string>> Messages { get; set; }

        public string DiscountMessage { get; set; }

        public string DiscountDescription { get; set; }



        public IList<KeyValuePair<string, string>> BillToAddress { get; set; }

        public IList<KeyValuePair<string, string>> CreditCardDetails { get; set; }

        public IList<ReviewUpdateOrderModel> Shipments { get; set; }

        public MultipleOrdersReviewModel()
        {
            GiftCertificateCodes = new List<CartGcModel>();
            Shipments = new List<ReviewUpdateOrderModel>();
        }
    }
}
