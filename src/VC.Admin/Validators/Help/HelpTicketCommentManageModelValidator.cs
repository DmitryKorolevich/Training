using FluentValidation;
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
    public class HelpTicketCommentManageModelValidator : ModelValidator<HelpTicketCommentManageModel>
    {
        public override void Validate(HelpTicketCommentManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<HelpTicketCommentModelValidator>().Validate(value));
        }

        private class HelpTicketCommentModelValidator : AbstractValidator<HelpTicketCommentManageModel>
        {
            public HelpTicketCommentModelValidator()
            {
                RuleFor(model => model.IdHelpTicket)
                    .NotEmpty()
                    .WithMessage(model => model.IdHelpTicket, ValidationMessages.FieldRequired);
                
                RuleFor(model => model.Comment)
                    .NotEmpty()
                    .WithMessage(model => model.Comment, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Comment, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}