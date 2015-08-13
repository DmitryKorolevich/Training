using System;
using System.Linq;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class CustomerModelConverter : IModelToDynamicConverter<AddUpdateCustomerModel, CustomerDynamic>
    {
	    private readonly IDynamicToModelMapper<CustomerNoteDynamic> _customerNoteMapper;
        private readonly IDynamicToModelMapper<CustomerPaymentMethodDynamic> _paymentMethodMapper;
        private readonly IDynamicToModelMapper<AddressDynamic> _addressMapper;

        public CustomerModelConverter(IDynamicToModelMapper<CustomerNoteDynamic> customerNoteMapper,
            IDynamicToModelMapper<AddressDynamic> addressMapper,
            IDynamicToModelMapper<CustomerPaymentMethodDynamic> paymentMethodMapper)
        {
            _customerNoteMapper = customerNoteMapper;
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        public void DynamicToModel(AddUpdateCustomerModel model, CustomerDynamic dynamic)
	    {
		    if (dynamic.CustomerNotes.Any())
		    {
			    foreach (var customerNote in dynamic.CustomerNotes)
			    {
					model.CustomerNotes.Add(_customerNoteMapper.ToModel<CustomerNoteModel>(customerNote));
                }
		    }

		    if (dynamic.Addresses.Any())
		    {
			    foreach (var address in dynamic.Addresses)
			    {
				    switch (address.IdObjectType)
				    {
					    case (int)AddressType.Shipping:
						    model.Shipping.Add(_addressMapper.ToModel<AddressModel>(address));
						    break;
					    case (int)AddressType.Profile:
						    model.ProfileAddress = _addressMapper.ToModel<AddressModel>(address);
						    break;
				    }
			    }
		    }
	        var oacPaymentType =
	            dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int) PaymentMethodType.Oac);
	        var checkType =
	            dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int) PaymentMethodType.Check);
	        if (oacPaymentType != null)
	        {
	            model.Oac = _paymentMethodMapper.ToModel<OacPaymentModel>(oacPaymentType);
	        }
	        if (checkType != null)
	        {
	            model.Check = _paymentMethodMapper.ToModel<CheckPaymentModel>(checkType);
	        }
	        foreach (var creditCard in dynamic.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard))
	        {
	            model.CreditCards.Add(_paymentMethodMapper.ToModel<CreditCardModel>(creditCard));
	        }
            model.SuspendUserAccount = dynamic.StatusCode == RecordStatusCode.NotActive;

			if (dynamic.Files.Any())
			{
				foreach (var fileDynamic in dynamic.Files.Select(x => new CustomerFileModel()
				{
					Id = x.Id,
					Description = x.Description,
					FileName = x.FileName,
					UploadDate = x.UploadDate
				}))
				{
					model.Files.Add(fileDynamic);
				}
			}
		}

	    public void ModelToDynamic(AddUpdateCustomerModel model, CustomerDynamic dynamic)
        {
			if (model.CustomerNotes.Any() && !string.IsNullOrWhiteSpace(model.CustomerNotes[0].Text))
			{
				foreach (var customerNoteDynamic in model.CustomerNotes.Select(customerNote => _customerNoteMapper.FromModel(customerNote)))
				{
					dynamic.CustomerNotes.Add(customerNoteDynamic);
				}
			}

			if (model.ProfileAddress != null)
			{
				var addressDynamic = _addressMapper.FromModel(model.ProfileAddress);
				addressDynamic.IdObjectType = (int)AddressType.Profile;
				dynamic.Addresses.Add(addressDynamic);
			}
			if (model.Shipping.Any())
			{
				foreach (var addressDynamic in model.Shipping.Select(shipping => _addressMapper.FromModel(shipping)))
				{
					addressDynamic.IdObjectType = (int)AddressType.Shipping;
					dynamic.Addresses.Add(addressDynamic);
				}
			}
	        foreach (var creditCard in model.CreditCards)
	        {
                creditCard.PaymentMethodType = PaymentMethodType.CreditCard;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(creditCard));
	        }
	        if (model.Oac?.Address != null)
	        {
                model.Oac.PaymentMethodType = PaymentMethodType.Oac;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(model.Oac));
	        }
            if (model.Check?.Address != null)
            {
                model.Check.PaymentMethodType = PaymentMethodType.Check;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(model.Check));
            }
            dynamic.StatusCode = model.SuspendUserAccount ? RecordStatusCode.NotActive : RecordStatusCode.Active;

			if (model.Files.Any())
			{
				foreach (var fileModel in model.Files.Select(x => new CustomerFile()
				{
					Id = x.Id,
					Description = x.Description,
					FileName = x.FileName,
					IdCustomer = model.Id,
					UploadDate = x.UploadDate
				}))
				{
					dynamic.Files.Add(fileModel);
				}
			}
		}
	}
}
