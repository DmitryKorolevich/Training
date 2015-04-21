using System;
#if DNX451
using System.ComponentModel;
#endif

namespace VitalChoice.Domain.Entities.Users
{
    public enum UserStatus
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