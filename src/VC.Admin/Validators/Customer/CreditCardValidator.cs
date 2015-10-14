using System;
using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;

namespace VC.Admin.Validators.Customer
{
    public class CreditCardValidator : ModelValidator<CreditCardModel>
    {
        public override void Validate(CreditCardModel value)
        {
            ValidationErrors.Clear();
            var creditCardValidator = ValidatorsFactory.GetValidator<CreditCardModelRules>();
            ParseResults(creditCardValidator.Validate(value));

            var billingAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(billingAddressValidator.Validate(value.Address));
        }
    }

    public class CreditCardModelRules : AbstractValidator<CreditCardModel>
    {
        public CreditCardModelRules()
        {
            RuleFor(model => model.CardNumber)
                .NotEmpty()
                .WithMessage(model => model.CardNumber, ValidationMessages.FieldRequired)
                .Length(BaseAppConstants.CREDIT_CARD_MAX_LENGTH, BaseAppConstants.CREDIT_CARD_MAX_LENGTH)
                .When(model => model.CardType != CreditCardType.AmericanExpress)
                .WithMessage(model => model.CardNumber, ValidationMessages.FieldLength,
                    BaseAppConstants.CREDIT_CARD_MAX_LENGTH)
                .Length(BaseAppConstants.CREDIT_CARD_AM_EXPRESS_MAX_LENGTH, BaseAppConstants.CREDIT_CARD_AM_EXPRESS_MAX_LENGTH)
                .When(model => model.CardType == CreditCardType.AmericanExpress)
                .WithMessage(model => model.CardNumber, ValidationMessages.FieldLength,
                    BaseAppConstants.CREDIT_CARD_AM_EXPRESS_MAX_LENGTH);

            RuleFor(model => model.CardType)
                .NotEmpty()
                .WithMessage(model => model.CardType, ValidationMessages.FieldRequired);

            RuleFor(model => model.ExpirationDateMonth)
                .NotEmpty()
                .WithMessage(model => model.ExpirationDateMonth, ValidationMessages.FieldRequired)
                .Must(model => model <= 12 && model >= 1)
                .WithMessage(model => model.ExpirationDateMonth, ValidationMessages.MonthFormat);

            RuleFor(model => model.ExpirationDateYear)
                .NotEmpty()
                .WithMessage(model => model.ExpirationDateYear, ValidationMessages.FieldRequired)
                .Must(model => model <= 99 && model >= (DateTime.Now.Year-2000))
                .WithMessage(model => model.ExpirationDateYear, ValidationMessages.YearFormat);

            RuleFor(model => model)
                .Must(model => (new DateTime(model.ExpirationDateYear.Value+2000, model.ExpirationDateMonth.Value,1)).AddMonths(1)>DateTime.Now)
                .When(model => model.ExpirationDateMonth <= 12 && model.ExpirationDateMonth >= 1 && model.ExpirationDateYear <= 99 && model.ExpirationDateYear >= (DateTime.Now.Year - 2000))
                .WithMessage("Expiration Date should be future date")
                .WithName("ExpirationDateMonth");

            RuleFor(model => model.NameOnCard)
                .NotEmpty()
                .WithMessage(model => model.NameOnCard, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .WithMessage(model => model.NameOnCard, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
        }
    }
}
