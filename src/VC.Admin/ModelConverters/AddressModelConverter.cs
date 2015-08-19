using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class AddressModelConverter : IModelToDynamicConverter<AddressModel, CustomerAddressDynamic>
    {
	    public void DynamicToModel(AddressModel model, CustomerAddressDynamic dynamic)
	    {
		    model.Country.Id = dynamic.IdCountry;
	    }

	    public void ModelToDynamic(AddressModel model, CustomerAddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	    }
	}
}
