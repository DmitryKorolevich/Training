using VC.Public.Models.Auth;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Public.ModelConverters
{
    public class CusomerAddressModelConverter : IModelToDynamicConverter<RegisterAccountModel, CustomerAddressDynamic>
    {
	    public void DynamicToModel(RegisterAccountModel model, CustomerAddressDynamic dynamic)
	    {
		    model.IdCountry = dynamic.IdCountry;
	    }

	    public void ModelToDynamic(RegisterAccountModel model, CustomerAddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.IdCountry;
	    }
	}
}
