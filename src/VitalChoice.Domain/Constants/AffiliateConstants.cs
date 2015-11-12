using System;

namespace VitalChoice.Domain.Constants
{
    public class AffiliateConstants
    {
        public const decimal DefaultCommissionFirst = 8.00m;
        public const decimal DefaultCommissionAll = 5.00m;
        public const int DefaultPaymentType = 2;
        public const int DefaultTier = 1;
        
        public const string AffiliatePublicIdParam = "idaffiliate";
        public const int AffiliatePublicIdParamExpiredDays = 30;
        
        public const decimal AffiliateMinPayCommisionsAmount = 50;
        public const string AffiliateOrderPaymentsCountToDateOptionName = "AffiliateOrderPaymentsCountToDate";
    }
}