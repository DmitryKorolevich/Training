namespace VitalChoice.Infrastructure.Domain.Entities.Roles
{
    public enum RoleType
    {
        /*Admin*/
        AdminUser = 1,
		ContentUser = 2,
		OrderUser = 3,
		ProductUser = 4,
		SuperAdminUser = 5,

		/*Customers*/
		Retail = 6,
		Wholesale = 7,

        /*Affiliates*/
        Affiliate = 8
	}
}