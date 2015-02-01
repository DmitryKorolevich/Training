using System.Text.RegularExpressions;
using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class YoutubeURLValidator : AbstractValidator<string>
    {
        public YoutubeURLValidator()
        {
            RuleFor(model => model).Cascade(CascadeMode.Continue)
                .Matches(ValidationPatterns.YouTubeURLValidPatter).When(p => (new Regex(ValidationPatterns.YouTubeURLValidPatter)).Match(p).Success)
                .WithMessage("YouTubeURLInvalid", ValidationScope.Api)
                .WithName("YouTubeURL");
            RuleFor(model => model).Cascade(CascadeMode.Continue)
                .Matches(ValidationPatterns.YouTubeShortURLValidPatter).When(p => (new Regex(ValidationPatterns.YouTubeShortURLValidPatter)).Match(p).Success)
                .WithMessage("YouTubeURLInvalid", ValidationScope.Api)
                .WithName("YouTubeURL");
        }
    }
}