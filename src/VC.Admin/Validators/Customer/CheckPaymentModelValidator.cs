using FluentValidation;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Customer
{
    public class CheckPaymentModelValidator : ModelValidator<CheckPaymentModel>
    {
        public override void Validate(CheckPaymentModel value)
        {
            ValidationErrors.Clear();
            var creditCardValidator = ValidatorsFactory.GetValidator<CheckPaymentModelRules>();
            ParseResults(creditCardValidator.Validate(value));

            var billingAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(billingAddressValidator.Validate(value.Address));
        }
    }

    public class CheckPaymentModelRules : AbstractValidator<CheckPaymentModel>
    {
        public CheckPaymentModelRules()
        {
            RuleFor(model => model.CheckNumber)
                .NotEmpty()
                .WithMessage(model => model.CheckNumber, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.CheckNumber, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
        }
    }
}
