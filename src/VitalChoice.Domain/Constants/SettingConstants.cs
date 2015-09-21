using System;

namespace VitalChoice.Domain.Constants
{
    public class SettingConstants
    {
        public const string GLOBAL_PERISHABLE_THRESHOLD_NAME = "GlobalPerishableThreshold";
        public const string GLOBAL_CREDIT_CARD_AUTHORIZATIONS_NAME = "CreditCardAuthorizations";

        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE = "ProductOutOfStockEmailTemplate";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_CUSTOMER_NAME_HOLDER = "{CUSTOMER_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_NAME_HOLDER = "{PRODUCT_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_URL_HOLDER = "{PRODUCT_URL}";
    }
}