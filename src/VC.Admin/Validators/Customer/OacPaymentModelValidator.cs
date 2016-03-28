using FluentValidation;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Customer
{
    public class OacPaymentModelValidator : ModelValidator<OacPaymentModel>
    {
        public override void Validate(OacPaymentModel value)
        {
            ValidationErrors.Clear();
            var creditCardValidator = ValidatorsFactory.GetValidator<OacPaymentModelRules>();
            ParseResults(creditCardValidator.Validate(value));

            var billingAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(billingAddressValidator.Validate(value.Address));
        }
    }

    public class OacPaymentModelRules : AbstractValidator<OacPaymentModel>
    {
        public OacPaymentModelRules()
        {
            RuleFor(model => model.Terms)
                .NotEmpty()
                .WithMessage(model => model.Terms, ValidationMessages.FieldRequired);

            RuleFor(model => model.Fob)
                .NotEmpty()
                .WithMessage(model => model.Fob, ValidationMessages.FieldRequired);
        }
    }
}
