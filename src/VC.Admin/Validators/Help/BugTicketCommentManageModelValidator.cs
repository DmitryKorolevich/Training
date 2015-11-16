using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Admin.Models.Help;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Affiliate
{
    public class BugTicketCommentManageModelValidator : ModelValidator<BugTicketCommentManageModel>
    {
        public override void Validate(BugTicketCommentManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<BugTicketCommentModelValidator>().Validate(value));
        }

        private class BugTicketCommentModelValidator : AbstractValidator<BugTicketCommentManageModel>
        {
            public BugTicketCommentModelValidator()
            {
                RuleFor(model => model.IdBugTicket)
                    .NotEmpty()
                    .WithMessage(model => model.IdBugTicket, ValidationMessages.FieldRequired);
                
                RuleFor(model => model.Comment)
                    .NotEmpty()
                    .WithMessage(model => model.Comment, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Comment, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}