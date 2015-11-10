using System;

namespace VitalChoice.Infrastructure.Identity
{
    public static class IdentityConstants
    {
	    public const string PermissionRoleClaimType = "Permission";

	    public const string CustomerRoleType = "CustomerRole";

        public const string AffiliateRole = "AffiliateRole";

        public const string IdentityBasicProfile = "Basic";

	    public const string RetailCustomer = "Retail";

	    public const string WholesaleCustomer = "Wholesale";

        public const string ForgotPasswordResetPurpose = "ForgotPasswordReset";

        public const string PasswordResetPurpose = "ResetPassword";

        public const string CustomerLoginPurpose = "CustomerLoginPurpose";

        public const string TokenProviderName = "Default";
    }
}