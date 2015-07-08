using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class AddressModelConverter : IModelToDynamicConverter<AddressModel, AddressDynamic>
    {
	    public void DynamicToModel(AddressModel model, AddressDynamic dynamic)
	    {
	    }

	    public void ModelToDynamic(AddressModel model, AddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	    }
	}
}
