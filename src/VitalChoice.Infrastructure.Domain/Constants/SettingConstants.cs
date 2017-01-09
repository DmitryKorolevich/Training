namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class SettingConstants
    {
        public const string GLOBAL_PERISHABLE_THRESHOLD_NAME = "GlobalPerishableThreshold";
        public const string GLOBAL_CREDIT_CARD_AUTHORIZATIONS_NAME = "CreditCardAuthorizations";

        public const string TEMPLATE_PUBLIC_URL_HOLDER = "{PUBLIC_URL}";

        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE = "ProductOutOfStockEmailTemplate";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_CUSTOMER_NAME_HOLDER = "{CUSTOMER_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_NAME_HOLDER = "{PRODUCT_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_URL_HOLDER = "{PRODUCT_URL}";

        public const string AFFILIATE_EMAIL_TEMPLATE = "AffiliateEmailTemplate";
        public const string AFFILIATE_EMAIL_TEMPLATE_DR_SEARS = "AffiliateEmailTemplateDrSears";
        public const string AFFILIATE_EMAIL_TEMPLATE_NAME_HOLDER = "{AFFILIATE_NAME}";
        public const string AFFILIATE_EMAIL_TEMPLATE_ID_HOLDER = "{AFFILIATE_ID}";
        public const string AFFILIATE_EMAIL_TEMPLATE_EMAIL_HOLDER = "{AFFILIATE_EMAIL}";

        public const string SETTINGS_LOOKUP_NAMES = "ServiceCodes,InventorySkuChannels,InventorySkuProductSources," +
                                                    "InventorySkuUnitOfMeasures,InventorySkuPurchaseUnitOfMeasures,EmailOrderRequestors,EmailOrderReasons";
        public const string INVENTORY_SKU_LOOKUP_CHANNEL_NAME = "InventorySkuChannels";
        public const string INVENTORY_SKU_LOOKUP_PRODUCT_SOURCE_NAME = "InventorySkuProductSources";
        public const string INVENTORY_SKU_LOOKUP_UNIT_OF_MEASURE_NAME = "InventorySkuUnitOfMeasures";
        public const string INVENTORY_SKU_LOOKUP_PURCHASE_UNIT_OF_MEASURE_NAME = "InventorySkuPurchaseUnitOfMeasures";
        public const string INVENTORY_SKU_LOOKUP_NAMES = "InventorySkuChannels,InventorySkuProductSources,InventorySkuUnitOfMeasures,InventorySkuPurchaseUnitOfMeasures";
        public const string SERVICE_CODE_LOOKUP_NAME = "ServiceCodes";
        public const string EMAIL_ORDER_LOOKUP_NAMES = "EmailOrderRequestors,EmailOrderReasons";
        public const string EMAIL_ORDER_REQUESTOR_LOOKUP_NAME = "EmailOrderRequestors";
        public const string EMAIL_ORDER_REASON_LOOKUP_NAME = "EmailOrderReasons";
    }
}