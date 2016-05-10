using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
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

        public override async Task DynamicToModelAsync(AddUpdateCustomerModel model, CustomerDynamic dynamic)
	    {
		    if (dynamic.CustomerNotes.Any())
		    {
			    foreach (var customerNote in dynamic.CustomerNotes)
			    {
					model.CustomerNotes.Add(await _customerNoteMapper.ToModelAsync<CustomerNoteModel>(customerNote));
                }
		    }
            model.ProfileAddress = await _addressMapper.ToModelAsync<AddressModel>(dynamic.ProfileAddress);
            if (dynamic.ShippingAddresses.Any())
            {
                foreach (var address in dynamic.ShippingAddresses)
                {
                    model.Shipping.Add(await _addressMapper.ToModelAsync<AddressModel>(address));
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
	        model.Oac = await _paymentMethodMapper.ToModelAsync<OacPaymentModel>(oacPaymentType);
	        model.Check = await _paymentMethodMapper.ToModelAsync<CheckPaymentModel>(checkType);
	        foreach (var creditCard in dynamic.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard))
	        {
	            model.CreditCards.Add(await _paymentMethodMapper.ToModelAsync<CreditCardModel>(creditCard));
	        }
            model.WireTransfer = await _paymentMethodMapper.ToModelAsync<WireTransferPaymentModel>(wireTransferType);
            model.Marketing = await _paymentMethodMapper.ToModelAsync<MarketingPaymentModel>(marketingType);
            model.VCWellness = await _paymentMethodMapper.ToModelAsync<VCWellnessEmployeeProgramPaymentModel>(vcWellnessType);

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

	    public override async Task ModelToDynamicAsync(AddUpdateCustomerModel model, CustomerDynamic dynamic)
        {
			if (model.CustomerNotes.Any() && !string.IsNullOrWhiteSpace(model.CustomerNotes[0].Text))
			{
				foreach (var customerNoteDynamic in await Task.WhenAll(model.CustomerNotes.Select(async customerNote => await _customerNoteMapper.FromModelAsync(customerNote))))
				{
					dynamic.CustomerNotes.Add(customerNoteDynamic);
				}
			}

	        if (model.ProfileAddress != null)
	        {
	            dynamic.ProfileAddress = await _addressMapper.FromModelAsync(model.ProfileAddress, (int)AddressType.Profile);
	            dynamic.ProfileAddress.Data.Email = model.Email;
	        }
	        if (model.Shipping.Any())
			{
				foreach (var addressDynamic in await Task.WhenAll(model.Shipping.Select(async shipping => await _addressMapper.FromModelAsync(shipping, (int)AddressType.Shipping))))
				{
					dynamic.ShippingAddresses.Add(addressDynamic);
				}
			}
	        foreach (var creditCard in model.CreditCards)
	        {
                creditCard.PaymentMethodType = PaymentMethodType.CreditCard;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(creditCard));
	        }
	        if (model.Oac?.Address != null)
	        {
                model.Oac.PaymentMethodType = PaymentMethodType.Oac;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(model.Oac));
	        }
            if (model.Check?.Address != null)
            {
                model.Check.PaymentMethodType = PaymentMethodType.Check;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(model.Check));
            }
            if (model.WireTransfer?.Address != null)
            {
                model.WireTransfer.PaymentMethodType = PaymentMethodType.WireTransfer;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(model.WireTransfer));
            }
            if (model.Marketing?.Address != null)
            {
                model.Marketing.PaymentMethodType = PaymentMethodType.Marketing;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(model.Marketing));
            }
            if (model.VCWellness?.Address != null)
            {
                model.VCWellness.PaymentMethodType = PaymentMethodType.VCWellnessEmployeeProgram;
                dynamic.CustomerPaymentMethods.Add(await _paymentMethodMapper.FromModelAsync(model.VCWellness));
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
                dynamic.Files = dynamic.Files ?? new List<CustomerFile>();
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
