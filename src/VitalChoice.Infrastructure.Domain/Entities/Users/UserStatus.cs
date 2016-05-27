using System.ComponentModel;
#if !NETSTANDARD1_5

#endif

namespace VitalChoice.Infrastructure.Domain.Entities.Users
{
    public enum UserStatus :byte
    {
#if !NETSTANDARD1_5
		[Description("Not Active")]
#endif
		NotActive = 0,
#if !NETSTANDARD1_5
		[Description("Active")]
#endif
		Active = 1,
#if !NETSTANDARD1_5
		[Description("Disabled")]
#endif
		Disabled = 2

	}
}