using System.ComponentModel;
#if DNX451

#endif

namespace VitalChoice.Infrastructure.Domain.Entities.Users
{
    public enum UserStatus :byte
    {
#if DNX451
		[Description("Not Active")]
#endif
		NotActive = 0,
#if DNX451
		[Description("Active")]
#endif
		Active = 1,
#if DNX451
		[Description("Disabled")]
#endif
		Disabled = 2

	}
}