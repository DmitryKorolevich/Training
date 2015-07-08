using System;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class CustomerModelConverter : IModelToDynamicConverter<AddUpdateCustomerModel, CustomerDynamic>
    {
	    private readonly IDynamicToModelMapper<CustomerNoteDynamic> _customerNoteMapper;
	    private readonly IDynamicToModelMapper<AddressDynamic> _addressMapper;

		public CustomerModelConverter(IDynamicToModelMapper<CustomerNoteDynamic> customerNoteMapper, IDynamicToModelMapper<AddressDynamic> addressMapper)
		{
			_customerNoteMapper = customerNoteMapper;
			_addressMapper = addressMapper;
		}

	    public void DynamicToModel(AddUpdateCustomerModel model, CustomerDynamic dynamic)
	    {
	    }

	    public void ModelToDynamic(AddUpdateCustomerModel model, CustomerDynamic dynamic)
        {
			if (model.CustomerNote != null)
			{
				var customerNoteDynamic = _customerNoteMapper.FromModel(model.CustomerNote);
                dynamic.CustomerNotes.Add(customerNoteDynamic);
			}

			if (model.ProfileAddress != null)
			{
				var addressDynamic = _addressMapper.FromModel(model.ProfileAddress);
				addressDynamic.IdObjectType = (int)AddressType.Profile;
				dynamic.Addresses.Add(addressDynamic);
			}

			if (model.Shipping != null)
			{
				var addressDynamic = _addressMapper.FromModel(model.Shipping);
				addressDynamic.IdObjectType = (int)AddressType.Shipping;
				dynamic.Addresses.Add(addressDynamic);
			}
		}
	}
}
