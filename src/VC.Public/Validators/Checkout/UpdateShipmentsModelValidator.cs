using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Public.Models.Affiliate;
using System;
using VC.Public.Models.Checkout;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Public.Validators.Checkout
{
    public class UpdateShipmentsModelValidator : ModelValidator<UpdateShipmentsModel>
    {
        public override void Validate(UpdateShipmentsModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<UpdateShipmentsModelInnerValidator>().Validate(value));
            if (value.Shipments != null)
            {
                for (int i = 0; i < value.Shipments.Count; i++)
                {
                    ParseResults(ValidatorsFactory.GetValidator<ShippingAddressModelInnerValidator>().Validate(value.Shipments[i]), null, i);
                }
            }
        }

        private class UpdateShipmentsModelInnerValidator : AbstractValidator<UpdateShipmentsModel>
        {
            public UpdateShipmentsModelInnerValidator()
            {
                RuleFor(model => model.Email)
                  .NotEmpty()
                  .When(p => p.CreateAccount)
                  .WithMessage(model => model.Email, ValidationMessages.FieldRequired)
                  .EmailAddress()
                  .When(p => p.CreateAccount)
                  .WithMessage(model => model.Email, ValidationMessages.EmailFormat)
                  .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                  .WithMessage(model => model.Email, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Password)
                  .NotEmpty()
                  .When(p => p.CreateAccount && !p.GuestCheckout)
                  .WithMessage(model => model.Password, ValidationMessages.FieldRequired)
                  .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                  .WithMessage(model => model.Password, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.ConfirmPassword)
                  .NotEmpty()
                  .When(p => p.CreateAccount && !p.GuestCheckout)
                  .WithMessage(model => model.ConfirmPassword, ValidationMessages.FieldRequired)
                  .Must((p, a) => a == p.ConfirmPassword)
                  .When(p => !string.IsNullOrEmpty(p.ConfirmPassword) && p.CreateAccount && !p.GuestCheckout)
                  .WithCustomMessage(model => model.ConfirmPassword, "'Password Confirm' and 'Password' do not match.")
                  .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                  .WithMessage(model => model.ConfirmPassword, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }

        private class ShippingAddressModelInnerValidator : AbstractValidator<ShippingAddressModel>
        {
            public ShippingAddressModelInnerValidator()
            {
                RuleFor(model => model.FirstName)
                 .NotEmpty()
                 .When(p=>!p.UseBillingAddress)
                 .WithMessage(model => model.FirstName, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.FirstName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.LastName)
                 .NotEmpty()
                 .When(p => !p.UseBillingAddress)
                 .WithMessage(model => model.LastName, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.LastName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Company)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.Company, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.IdCountry)
                    .Must(p => p != 0)
                    .When(p => !p.UseBillingAddress)
                    .WithMessage(model => model.IdCountry, ValidationMessages.FieldRequired);

                RuleFor(model => model.County)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.County, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Address1)
                 .NotEmpty()
                 .When(p => !p.UseBillingAddress)
                 .WithMessage(model => model.Address1, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.Address1, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Address2)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.Address2, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.City)
                 .NotEmpty()
                 .When(p => !p.UseBillingAddress)
                 .WithMessage(model => model.City, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.City, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.PostalCode)
                 .NotEmpty()
                 .When(p => !p.UseBillingAddress)
                 .WithMessage(model => model.PostalCode, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.ZIP_MAX_SIZE)
                 .WithMessage(model => model.PostalCode, ValidationMessages.FieldLength, BaseAppConstants.ZIP_MAX_SIZE);

                RuleFor(model => model.Phone)
                 .NotEmpty()
                 .When(p => !p.UseBillingAddress)
                 .WithMessage(model => model.Phone, ValidationMessages.FieldRequired)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.Phone, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Fax)
                 .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                 .WithMessage(model => model.Fax, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}