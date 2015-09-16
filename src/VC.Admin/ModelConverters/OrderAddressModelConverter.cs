using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class OrderAddressModelConverter : IModelToDynamicConverter<AddressModel, OrderAddressDynamic>
    {
	    public void DynamicToModel(AddressModel model, OrderAddressDynamic dynamic)
	    {
		    model.Country.Id = dynamic.IdCountry;
	    }

	    public void ModelToDynamic(AddressModel model, OrderAddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	    }
	}
}
