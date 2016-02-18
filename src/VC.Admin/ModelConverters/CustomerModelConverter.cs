using System.Linq;
using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class CustomerModelConverter : BaseModelConverter<AddUpdateCustomerModel, CustomerDynamic>
    {
	    private readonly IDynamicMapper<CustomerNoteDynamic, CustomerNote> _customerNoteMapper;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodMapper;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;

        public CustomerModelConverter(IDynamicMapper<CustomerNoteDynamic, CustomerNote> customerNoteMapper,
            IDynamicMapper<AddressDynamic, Address> addressMapper,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodMapper)
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
            model.ProfileAddress = _addressMapper.ToModel<AddressModel>(dynamic.ProfileAddress);
            if (dynamic.ShippingAddresses.Any())
            {
                foreach (var address in dynamic.ShippingAddresses)
                {
                    model.Shipping.Add(_addressMapper.ToModel<AddressModel>(address));
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
            var vcWellnessType =
                dynamic.CustomerPaymentMethods.SingleOrDefault(p => p.IdObjectType == (int)PaymentMethodType.VCWellnessEmployeeProgram);
	        model.Oac = _paymentMethodMapper.ToModel<OacPaymentModel>(oacPaymentType);
	        model.Check = _paymentMethodMapper.ToModel<CheckPaymentModel>(checkType);
	        foreach (var creditCard in dynamic.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard))
	        {
	            model.CreditCards.Add(_paymentMethodMapper.ToModel<CreditCardModel>(creditCard));
	        }
            model.WireTransfer = _paymentMethodMapper.ToModel<WireTransferPaymentModel>(wireTransferType);
            model.Marketing = _paymentMethodMapper.ToModel<MarketingPaymentModel>(marketingType);
            model.VCWellness = _paymentMethodMapper.ToModel<VCWellnessEmployeeProgramPaymentModel>(vcWellnessType);

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
	            dynamic.ProfileAddress = _addressMapper.FromModel(model.ProfileAddress, (int)AddressType.Profile);
	            dynamic.ProfileAddress.Data.Email = model.Email;
	        }
	        if (model.Shipping.Any())
			{
				foreach (var addressDynamic in model.Shipping.Select(shipping => _addressMapper.FromModel(shipping, (int)AddressType.Shipping)))
				{
					dynamic.ShippingAddresses.Add(addressDynamic);
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
	        foreach (var customerPaymentMethodDynamic in dynamic.CustomerPaymentMethods)
	        {
	            if (customerPaymentMethodDynamic.Address != null)
	            {
	                customerPaymentMethodDynamic.Address.IdObjectType = (int) AddressType.Billing;
	            }
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
