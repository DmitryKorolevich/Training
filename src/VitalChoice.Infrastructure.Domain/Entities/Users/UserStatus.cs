using System.ComponentModel;
#if NET451

#endif

namespace VitalChoice.Infrastructure.Domain.Entities.Users
{
    public enum UserStatus :byte
    {
#if NET451
		[Description("Not Active")]
#endif
		NotActive = 0,
#if NET451
		[Description("Active")]
#endif
		Active = 1,
#if NET451
		[Description("Disabled")]
#endif
		Disabled = 2

	}
}