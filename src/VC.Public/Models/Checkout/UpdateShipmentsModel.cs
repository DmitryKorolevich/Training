using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
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
        }

        public ICollection<AvalibleShippingAddressModel> AvalibleAddresses { get; set; }

        public IList<ShippingAddressModel> Shipments { get; set; }
    }
}
