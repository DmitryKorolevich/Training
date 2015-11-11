using System;
using System.Linq;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class CustomerModelConverter : BaseModelConverter<AddUpdateCustomerModel, CustomerDynamic>
    {
	    private readonly IDynamicMapper<CustomerNoteDynamic> _customerNoteMapper;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic> _paymentMethodMapper;
        private readonly IDynamicMapper<CustomerAddressDynamic> _addressMapper;

        public CustomerModelConverter(IDynamicMapper<CustomerNoteDynamic> customerNoteMapper,
            IDynamicMapper<CustomerAddressDynamic> addressMapper,
            IDynamicMapper<CustomerPaymentMethodDynamic> paymentMethodMapper)
        {
            _customerNoteMapper = customerNoteMapper;
            _addressMapper = addressMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        public override void DynamicToModel(AddUpdateCustomerModel model, CustomerDynamic dynamic)
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
            var wireTransferType =
                dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int)PaymentMethodType.WireTransfer);
            var marketingType =
                dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int)PaymentMethodType.Marketing);
            var VCWellnessType =
                dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int)PaymentMethodType.VCWellnessEmployeeProgram);
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
            if (wireTransferType != null)
            {
                model.WireTransfer = _paymentMethodMapper.ToModel<WireTransferPaymentModel>(wireTransferType);
            }
            if (marketingType != null)
            {
                model.Marketing = _paymentMethodMapper.ToModel<MarketingPaymentModel>(marketingType);
            }
            if (VCWellnessType != null)
            {
                model.VCWellness = _paymentMethodMapper.ToModel<VCWellnessEmployeeProgramPaymentModel>(VCWellnessType);
            }

            if (dynamic.Files!=null && dynamic.Files.Any())
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

	    public override void ModelToDynamic(AddUpdateCustomerModel model, CustomerDynamic dynamic)
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
				addressDynamic.Data.Email = model.Email;
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
            if (model.WireTransfer?.Address != null)
            {
                model.WireTransfer.PaymentMethodType = PaymentMethodType.WireTransfer;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(model.WireTransfer));
            }
            if (model.Marketing?.Address != null)
            {
                model.Marketing.PaymentMethodType = PaymentMethodType.Marketing;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(model.Marketing));
            }
            if (model.VCWellness?.Address != null)
            {
                model.VCWellness.PaymentMethodType = PaymentMethodType.VCWellnessEmployeeProgram;
                dynamic.CustomerPaymentMethods.Add(_paymentMethodMapper.FromModel(model.VCWellness));
            }

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
