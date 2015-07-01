using System.Linq;
using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Customer
{
	public class CustomerAddModelValidator : ModelValidator<AddCustomerModel>
	{
		public override void Validate(AddCustomerModel value)
		{
			ValidationErrors.Clear();

			var customerValidator = ValidatorsFactory.GetValidator<CustomerModelRules>();
			ParseResults(customerValidator.Validate(value));

			var profileAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(profileAddressValidator.Validate(value.ProfileAddress));
			ParseResults(profileAddressValidator.Validate(value.Shipping));

			var customerNoteValidator = ValidatorsFactory.GetValidator<CustomerNoteModelRules>();
			ParseResults(customerNoteValidator.Validate(value.CustomerNote));
		}

		private class CustomerModelRules : AbstractValidator<AddCustomerModel>
		{
			public CustomerModelRules()
			{
				RuleFor(model => model.Website)
					.NotEmpty()
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Website, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Website, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

				RuleFor(model => model.InceptionDate)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.InceptionDate, ValidationMessages.FieldRequired);

				RuleFor(model => model.TaxExempt)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.TaxExempt, ValidationMessages.FieldRequired);

				RuleFor(model => model.Tier)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Tier, ValidationMessages.FieldRequired);

				RuleFor(model => model.TradeClass)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.TradeClass, ValidationMessages.FieldRequired);

				RuleFor(model => model.LinkedToAffiliate)
					.NotEmpty()
					.WithMessage(model => model.LinkedToAffiliate, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.LinkedToAffiliate, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

				RuleFor(model => model.Email)
					.NotEmpty()
					.WithMessage(model => model.Email, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.Email, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.EmailAddress()
					.WithMessage(model => model.Email, ValidationMessages.EmailFormat);

				RuleFor(model => model.EmailConfirm).NotEmpty()
					.NotEmpty()
					.WithMessage(model => model.EmailConfirm, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.EmailConfirm, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.EmailAddress()
					.WithMessage(model => model.EmailConfirm, ValidationMessages.EmailFormat);

				RuleFor(model => model.Email)
					.Equal(x => x.EmailConfirm)
					.WithMessage(model => model.Email, ValidationMessages.EmailMustMatch);

				RuleFor(model => model.DefaultPaymentMethod)
					.Must(model => model.HasValue)
					.WithMessage(model => model.DefaultPaymentMethod, ValidationMessages.FieldRequired);

				RuleFor(model => model.Reason)
					.NotEmpty()
					.When(p => p.SuspendUserAccount)
					.WithMessage(model => model.Reason, ValidationMessages.FieldRequired)
					.Length(0, 1000)
					.When(p => p.SuspendUserAccount)
					.WithMessage(model => model.Reason, ValidationMessages.FieldLength, 1000);

				RuleFor(model => model.ApprovedPaymentMethods)
					.Must(model => model.Any())
					.WithMessage(model => model.ApprovedPaymentMethods, ValidationMessages.AtLeastOneItem);

				RuleFor(model => model.OrderNotes)
					.Must(model => model.Any())
					.WithMessage(model => model.OrderNotes, ValidationMessages.AtLeastOneItem);
			}
		}
	}
}