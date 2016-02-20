namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class ContentConstants
    {
        public const string CATEGORY_ID = "CategoryId";
        public const string NO_CATEGORIES_LABEL = "No Categories";
        public const string PREVIEW_PARAM = "preview";
        public const int ARTICLES_LIST_TAKE_COUNT = 50;
        public const int RECENT_ARTICLES_LIST_TAKE_COUNT = 5;
        public const int RECENT_RECIPES_FOR_ARTICLE_LIST_TAKE_COUNT = 1;
        
        public const string ARTICLE_BASE_URL = "/article/";
        public const string ARTICLE_CATEGORY_BASE_URL = "/articles/";

        public const string RECIPE_BASE_URL = "/recipe/";
        public const string RECIPE_CATEGORY_BASE_URL = "/recipes/";

        public const string FAQ_BASE_URL = "/faq/";
        public const string FAQ_CATEGORY_BASE_URL = "/faqs/";

        public const string FIELD_NAME_RELATED_RECIPE_IMAGE = "RelatedRecipeImage";
        public const string FIELD_NAME_RELATED_RECIPE_TITLE = "RelatedRecipeTitle";
        public const string FIELD_NAME_RELATED_RECIPE_URL = "RelatedRecipeUrl";

        public const string NOT_FOUND_PAGE_URL = "not-found";
        public const string ACESS_DENIED_PAGE_URL = "access-denied";
        public const string CONTENT_PAGE_TITLE_GENERAL_FORMAT = "{0} - Vital Choice Wild Seafood & Organics";

		public const int CONTENT_CROSS_SELL_LIMIT = 4;
	}
}