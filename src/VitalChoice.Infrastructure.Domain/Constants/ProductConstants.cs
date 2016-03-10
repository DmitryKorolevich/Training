namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class ProductConstants
    {
        public const string FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE = "CrossSellImage";
        public const string FIELD_NAME_CROSS_SELL_PRODUCT_URL = "CrossSellUrl";
        public const int FIELD_COUNT_CROSS_SELL_PRODUCT = 4;

        public const string FIELD_NAME_YOUTUBE_VIDEO = "YouTubeVideo";
        public const string FIELD_NAME_YOUTUBE_IMAGE = "YouTubeImage";
        public const string FIELD_NAME_YOUTUBE_TEXT = "YouTubeText";
        public const int FIELD_COUNT_YOUTUBE = 4;

        public const string FIELD_NAME_DISREGARD_STOCK = "DisregardStock";
        public const string FIELD_NAME_STOCK = "Stock";
        public const string FIELD_NAME_NON_DISCOUNTABLE = "NonDiscountable";
        public const string FIELD_NAME_HIDE_FROM_DATA_FEED = "HideFromDataFeed";
        public const string FIELD_NAME_QTY_THRESHOLD = "QTYThreshold";

        public const string FIELD_NAME_SKU_INVENTORY_BORN_DATE = "BornDate";

        public const string FIELD_NAME_INVENTORY_CATEGORY_ID = "InventoryCategoryId";

		public const int DEFAULT_FAVORITES_COUNT = 6;
		public const int MAX_FAVORITES_COUNT = 10000;
	}
}