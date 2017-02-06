using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VC.Public.Validators.Checkout;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AvalibleShippingAddressModel
    {
        public string Name { get; set; }

        public ShippingAddressModel Address { get; set; }
    }

    [ApiValidator(typeof(UpdateShipmentsModelValidator))]
    public class UpdateShipmentsModel : BaseModel
    {
        public UpdateShipmentsModel()
        {
            AvalibleAddresses = new List<AvalibleShippingAddressModel>();
            Shipments=new List<ShippingAddressModel>();
            SendNews = true;
            SendCatalog = false;
        }

        public bool AllowAddMultipleShipments { get; set; }

        public ICollection<AvalibleShippingAddressModel> AvalibleAddresses { get; set; }

        public IList<ShippingAddressModel> Shipments { get; set; }

        [DirectLocalized("Email")]
        [Map]
        public string Email { get; set; }

        public bool CreateAccount { get; set; }

        [DirectLocalized("Password")]
        [AllowXss]
        public string Password { get; set; }

        [DirectLocalized("Password Confirm")]
        [AllowXss]
        public string ConfirmPassword { get; set; }

        public bool GuestCheckout { get; set; }

        public bool SendNews { get; set; }

        public bool ShowSendCatalog { get; set; }

        public bool SendCatalog { get; set; }

        public int IdCustomerType { get; set; }
    }
}
