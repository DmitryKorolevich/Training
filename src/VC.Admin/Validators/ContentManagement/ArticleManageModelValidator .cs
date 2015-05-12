using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class ArticleManageModelValidator : ModelValidator<ArticleManageModel>
    {
        public override void Validate(ArticleManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ArticleModelValidator>().Validate(value));
        }

        private class ArticleModelValidator : AbstractValidator<ArticleManageModel>
        {
            public ArticleModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);
            }
        }
    }
}