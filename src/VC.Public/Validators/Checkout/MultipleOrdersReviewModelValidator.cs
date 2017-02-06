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
    public class MultipleOrdersReviewModelValidator : ModelValidator<MultipleOrdersReviewModel>
    {
        public override void Validate(MultipleOrdersReviewModel value)
        {
            ValidationErrors.Clear();
            if (value.Shipments != null)
            {
                for (int i = 0; i < value.Shipments.Count; i++)
                {
                    ParseResults(ValidatorsFactory.GetValidator<MultipleOrdersViewCartModelInnerValidator>().Validate(value.Shipments[i].OrderModel),
                        null, i);
                }
            }
        }

        private class MultipleOrdersViewCartModelInnerValidator : AbstractValidator<MultipleOrdersViewCartModel>
        {
            public MultipleOrdersViewCartModelInnerValidator()
            {
                RuleFor(model => model.ShippingDate)
                    .Must(p => p.HasValue)
                    .When(p => !p.ShipAsap)
                    .WithMessage(model => model.ShippingDate, ValidationMessages.FieldRequired);
                
                RuleFor(model => model.ShippingDate)
                    .Must(p => p.Value > DateTime.Today)
                    .When(p => p.ShippingDate.HasValue)
                    .WithCustomMessage(model => model.ShippingDate, "Shipping Date should be in the future");
            }
        }
    }
}