using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class CusomerAddressModelConverter : BaseModelConverter<AddressModel, AddressDynamic>
    {
	    public override void DynamicToModel(AddressModel model, AddressDynamic dynamic)
	    {
		    model.Country.Id = dynamic.IdCountry;
	    }

	    public override void ModelToDynamic(AddressModel model, AddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	    }
	}
}
