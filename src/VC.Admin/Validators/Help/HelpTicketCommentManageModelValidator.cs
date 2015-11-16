using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Admin.Models.Help;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

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