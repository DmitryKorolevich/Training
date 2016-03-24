namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class SettingConstants
    {
        public const string GLOBAL_PERISHABLE_THRESHOLD_NAME = "GlobalPerishableThreshold";
        public const string GLOBAL_CREDIT_CARD_AUTHORIZATIONS_NAME = "CreditCardAuthorizations";

        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE = "ProductOutOfStockEmailTemplate";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_CUSTOMER_NAME_HOLDER = "{CUSTOMER_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_NAME_HOLDER = "{PRODUCT_NAME}";
        public const string PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_URL_HOLDER = "{PRODUCT_URL}";

        public const string SETTINGS_LOOKUP_NAMES = "ServiceCodes,InventorySkuChannels,InventorySkuProductSources,InventorySkuUnitOfMeasures,InventorySkuPurchaseUnitOfMeasures";
        public const string INVENTORY_SKU_LOOKUP_CHANNEL_NAME = "InventorySkuChannels";
        public const string INVENTORY_SKU_LOOKUP_NAMES = "InventorySkuChannels,InventorySkuProductSources,InventorySkuUnitOfMeasures,InventorySkuPurchaseUnitOfMeasures";
        public const string SERVICE_CODE_LOOKUP_NAME = "ServiceCodes";
    }
}