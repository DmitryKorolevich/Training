using System.Linq;
using FluentValidation;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Validation.Logic;
using VC.Admin.Validators.Customer;
using System;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VC.Admin.Models.Orders;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Validators.Order
{
    public class OrderReshipManageModelValidator : ModelValidator<OrderReshipManageModel>
    {
        public override void Validate(OrderReshipManageModel value)
        {
            ValidationErrors.Clear();
            var inner = new OrderManageModelValidator();
            inner.Validate(value);
            ParseResults(inner);
        }
    }
}