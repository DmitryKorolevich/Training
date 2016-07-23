using System.Linq;
using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Customer
{
    public class AddressModelValidator : ModelValidator<AddressModel>
    {
        public override void Validate(AddressModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<AddressModelRules>().Validate(value));
        }
    }

    public class AddressModelRules : AbstractValidator<AddressModel>
    {
        public AddressModelRules()
        {
            RuleFor(model => model.Company)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Company, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.FirstName)
                .NotEmpty()
                .WithMessage(model => model.FirstName, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.FirstName, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.LastName)
                .NotEmpty()
                .WithMessage(model => model.LastName, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.LastName, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Address1)
                .NotEmpty()
                .WithMessage(model => model.Address1, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Address1, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Address2)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Address2, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.City)
                .NotEmpty()
                .WithMessage(model => model.City, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.City, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Country)
                .Must(model => model.Id != 0)
                .WithMessage(model => model.Country, ValidationMessages.FieldRequired);

            RuleFor(model => model.State)
                .Must(model => model != 0)
                .When(model => model.Country.States.Count > 0)
                .WithMessage(model => model.State, ValidationMessages.FieldRequired);

            //RuleFor(model => model.County)
            //	.NotEmpty()
            //	.When(model => !model.Country.States.Count > 0)
            //	.WithMessage(model => model.County, ValidationMessages.FieldRequired);

            RuleFor(model => model.Zip)
                .NotEmpty()
                .WithMessage(model => model.Zip, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Zip, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Phone)
                .NotEmpty()
                .WithMessage(model => model.Phone, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Phone, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Fax)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Fax, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Zip)
                .Length(0, BaseAppConstants.ZIP_MAX_SIZE)
                .WithMessage(model => model.Zip, ValidationMessages.FieldLength,
                    BaseAppConstants.ZIP_MAX_SIZE);

            RuleFor(model => model.Email)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Email, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(model => model.Email, ValidationMessages.EmailFormat);

            RuleFor(model => model.DeliveryInstructions)
                .Length(0, BaseAppConstants.DELIVERY_INSTRUCTIONS_MAX_SIZE)
                .WithMessage(model => model.DeliveryInstructions, ValidationMessages.FieldLength,
                    BaseAppConstants.DELIVERY_INSTRUCTIONS_MAX_SIZE);
        }

    }

    public class ProfileAddressModelRules : AbstractValidator<AddressModel>
    {
        public ProfileAddressModelRules()
        {
            RuleFor(model => model.Company)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Company, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.FirstName)
                .NotEmpty()
                .WithMessage(model => model.FirstName, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.FirstName, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.LastName)
                .NotEmpty()
                .WithMessage(model => model.LastName, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.LastName, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Phone)
                .NotEmpty()
                .WithMessage(model => model.Phone, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Phone, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Fax)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Fax, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.Zip)
                .Length(0, BaseAppConstants.ZIP_MAX_SIZE)
                .WithMessage(model => model.Zip, ValidationMessages.FieldLength,
                    BaseAppConstants.ZIP_MAX_SIZE);

            RuleFor(model => model.Email)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.Email, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(model => model.Email, ValidationMessages.EmailFormat);

            RuleFor(model => model.DeliveryInstructions)
                .Length(0, BaseAppConstants.DELIVERY_INSTRUCTIONS_MAX_SIZE)
                .WithMessage(model => model.DeliveryInstructions, ValidationMessages.FieldLength, BaseAppConstants.DELIVERY_INSTRUCTIONS_MAX_SIZE);
        }
    }
}