using System;

namespace VitalChoice.Domain.Entities.Roles
{
    public enum RoleType
    {
		/*Admin*/
		AdminUser = 1,
		ContentUser = 2,
		OrderUser = 3,
		ProductUser = 4,
		SuperAdminUser = 5,

		/*Public*/
		Retail = 6,
		Wholesale = 7,
		Affiliate = 8
	}
}