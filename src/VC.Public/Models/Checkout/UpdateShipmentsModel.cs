using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AvalibleShippingAddressModel
    {
        public string Name { get; set; }

        public ShippingAddressModel Address { get; set; }
    }

    public class UpdateShipmentsModel : BaseModel
    {
        public UpdateShipmentsModel()
        {
            AvalibleAddresses = new List<AvalibleShippingAddressModel>();
            Shipments=new List<ShippingAddressModel>();
        }

        public ICollection<AvalibleShippingAddressModel> AvalibleAddresses { get; set; }

        public ICollection<ShippingAddressModel> Shipments { get; set; }
    }
}
