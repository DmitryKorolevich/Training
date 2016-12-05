namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class FileConstants
    {
        public const int MAX_IMPORT_ROWS_COUNT = 10000;

        public const string VITAL_GREEN_REPORT_FILE_NAME_FORMAT = "VitalGreenReport_{0}_{1}.csv";
        public const string AFFILIATE_CUSTOMERS_REPORT = "AffiliateCustomersReport_{0}.csv";
        public const string CATALOG_REQUESTS_REPORT = "CatalogRequests_{0}.csv";
        public const string PRODUCT_CATEGORY_STATISTIC = "Category_Sales_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string REGIONAL_SALES_STATISTIC = "Regional_Sales_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string REGIONAL_SALES_DETAILS_STATISTIC = "Regional_Sales_Detail_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string GIFT_CERTIFICATES_REPORT_STATISTIC = "Gift_Certificates_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string REFUNDS_REPORT = "Refunds_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string RESHIPS_REPORT = "Reships_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string INVENTORY_SKUS_REPORT = "Parts_Usage_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string INVENTORY_SUMMARY_REPORT = "Product_Shipment_Summary_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string GOOGLE_PRODUCTS_FEED = "datafeed.csv";
        public const string CRITEO_PRODUCTS_FEED = "criteo-datafeed.csv";
        public const string CJ_PRODUCTS_FEED = "CJFEED.csv";
        public const string CJ_PRODUCTS_FEED_WITH_DATE_FORMAT = "CJFEED_{0:MM_dd_yy}.csv";

        public const string ORDERS_AGENT_REPORT = "Agents_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string WHOLESALE_DROPSHIP_REPORT = "Wholesale_Dropship_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string TRANSACTION_REFUND_REPORT = "Transaction_Refund_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string SUMMARY_SALES_REPORT = "Summary_Sales_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string WHOLESALE_LIST_REPORT = "Wholesale_List_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string SKU_BREAKDOWN_REPORT = "SKU_BreakDown_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string ORDERS_SKUS_REPORT = "Orders_Skus_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string MATCHBACK_REPORT = "Matchback_and_Post_Season_Analysis_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string MAILING_REPORT = "Mailing_List_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string SHIPPED_ORDERS_REPORT = "Shipped_Order_Listing_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string AAFES_SHIP_REPORT = "AAFES-Ship-Report-{0:MM-dd-yyyy-hh-mm-ss}.csv";
        public const string AFFILIATE_ORDER_STATUSES_REPORT = "Affiliate_Order_Status_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string SKU_PART_SUMMARY_LIST = "SKU_Part_Summary_List_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string CUSTOMER_SKU_USAGE_REPORT = "Customer_Sku_Usage_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string DISCOUNT_USAGE_REPORT = "Discount_Usage_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";

        public const string AVERAGE_DAILY_SALES_BY_SKU = "Average_Daily_Sales_By_Sku_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string AVERAGE_DAILY_SALES_BY_PRODUCT = "Average_Daily_Sales_By_Product_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string OOS_IMPACT_BY_SKU = "OOS_Impact_By_Sku_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
        public const string OOS_IMPACT_BY_PRODUCT = "OOS_Impact_By_Product_Report_{0:MM_dd_yyyy_hh_mm_ss}.csv";
    }
}