using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Domain.Constants;
using VC.Admin.Models.Order;

namespace VC.Admin.Validators.Order
{
    public class OrderManageModelValidator : ModelValidator<OrderManageModel>
    {
        public override void Validate(OrderManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value));
        }

        private class OrderModelValidator : AbstractValidator<OrderManageModel>
        {
            public OrderModelValidator()
            {
            }
        }
    }
}