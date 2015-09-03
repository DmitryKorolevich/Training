﻿using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;
using System;
using VC.Admin.Models.Help;

namespace VC.Admin.Validators.Affiliate
{
    public class HelpTicketManageModelValidator : ModelValidator<HelpTicketManageModel>
    {
        public override void Validate(HelpTicketManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<HelpTicketModelValidator>().Validate(value));
        }

        private class HelpTicketModelValidator : AbstractValidator<HelpTicketManageModel>
        {
            public HelpTicketModelValidator()
            {
                RuleFor(model => model.IdOrder)
                    .NotEmpty()
                    .WithMessage(model => model.IdOrder, ValidationMessages.FieldRequired);

                RuleFor(model => model.Priority)
                    .NotEmpty()
                    .WithMessage(model => model.Priority, ValidationMessages.FieldRequired);

                RuleFor(model => model.Summary)
                    .NotEmpty()
                    .WithMessage(model => model.Summary, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Summary, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Description, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}