using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class OrderAddressModelConverter : BaseModelConverter<AddressModel, OrderAddressDynamic>
    {
	    public override void DynamicToModel(AddressModel model, OrderAddressDynamic dynamic)
	    {
		    model.Country.Id = dynamic.IdCountry;
	    }

	    public override void ModelToDynamic(AddressModel model, OrderAddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	    }
	}
}
