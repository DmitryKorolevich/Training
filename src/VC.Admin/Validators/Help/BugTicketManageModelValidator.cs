using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Admin.Models.Help;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Affiliate
{
    public class BugTicketManageModelValidator : ModelValidator<BugTicketManageModel>
    {
        public override void Validate(BugTicketManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<BugTicketModelValidator>().Validate(value));
        }

        private class BugTicketModelValidator : AbstractValidator<BugTicketManageModel>
        {
            public BugTicketModelValidator()
            {
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