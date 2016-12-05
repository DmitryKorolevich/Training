using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace VC.Public
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                name: "Recipes_Categories",
                template: "recipes/",
                defaults: new { controller = "Recipe", action = "Categories" });

            routeBuilder.MapRoute(
                name: "Recipes_Category",
                template: "recipes/{url}",
                defaults: new { controller = "Recipe", action = "Category" });

            routeBuilder.MapRoute(
                name: "Recipes_Category_ByIdOld",
                template: "shop/pc/recipes.asp",
                defaults: new { controller = "Recipe", action = "CategoryByIdOld" });

            routeBuilder.MapRoute(
                name: "Recipes_Recipe",
                template: "recipe/{url}",
                defaults: new { controller = "Recipe", action = "Recipe" });

            routeBuilder.MapRoute(
                name: "Recipes_Recipe_ByIdOld",
                template: "shop/pc/viewrecipes.asp",
                defaults: new { controller = "Recipe", action = "RecipeByIdOld" });

            routeBuilder.MapRoute(
                name: "Recipes_Recipe_ByIdOld2",
                template: "shop/pc/viewrecipes_itk.asp",
                defaults: new { controller = "Recipe", action = "RecipeByIdOld" });

            routeBuilder.MapRoute(
                name: "FAQs_Categories",
                template: "faqs/",
                defaults: new { controller = "FAQ", action = "Categories" });

            routeBuilder.MapRoute(
                name: "FAQs_Category",
                template: "faqs/{url}",
                defaults: new { controller = "FAQ", action = "Category" });

            routeBuilder.MapRoute(
                name: "FAQs_Category_ByIdOld",
                template: "shop/pc/FAQView.asp",
                defaults: new { controller = "FAQ", action = "CategoryByIdOld" });

            routeBuilder.MapRoute(
                name: "FAQs_FAQ",
                template: "faq/{url}",
                defaults: new { controller = "FAQ", action = "FAQ" });

            routeBuilder.MapRoute(
                name: "FAQs_FAQ_ByIdOld",
                template: "shop/pc/FAQ_QView.asp",
                defaults: new { controller = "FAQ", action = "FAQByIdOld" });

            routeBuilder.MapRoute(
                name: "Articles_Categories",
                template: "articles/",
                defaults: new { controller = "Article", action = "Categories" });

            routeBuilder.MapRoute(
                name: "Articles_Category",
                template: "articles/{url}",
                defaults: new { controller = "Article", action = "Category" });

            routeBuilder.MapRoute(
                name: "Articles_Category_ByIdOld",
                template: "shop/pc/articles.asp",
                defaults: new { controller = "Article", action = "CategoryByIdOld" });

            routeBuilder.MapRoute(
                name: "Articles_Article",
                template: "article/{url}",
                defaults: new { controller = "Article", action = "Article" });

            routeBuilder.MapRoute(
                name: "Articles_Article_ByIdOld",
                template: "shop/pc/articlesView.asp",
                defaults: new { controller = "Article", action = "ArticleByIdOld" });

            routeBuilder.MapRoute(
                name: "ContentPages_Categories",
                template: "contents/",
                defaults: new { controller = "ContentPage", action = "Categories" });

            routeBuilder.MapRoute(
                name: "ContentPages_Category",
                template: "contents/{url}",
                defaults: new { controller = "ContentPage", action = "Category" });

            routeBuilder.MapRoute(
                name: "ContentPages_ContentPage",
                template: "content/{url}",
                defaults: new { controller = "ContentPage", action = "ContentPage" });

            routeBuilder.MapRoute(
                name: "ContentPages_ContentPage_ByIdOld",
                template: "shop/pc/viewContent.asp",
                defaults: new { controller = "ContentPage", action = "ContentPageByIdOld" });


            routeBuilder.MapRoute(
                name: "Products_Categories",
                template: "products/",
                defaults: new { controller = "Product", action = "Categories" });

            routeBuilder.MapRoute(
                name: "Products_Category",
                template: "products/{url}",
                defaults: new { controller = "Product", action = "Category" });

            routeBuilder.MapRoute(
                name: "Products_Category_ByIdOld",
                template: "shop/pc/viewCategories.asp",
                defaults: new { controller = "Product", action = "CategoryByIdOld" });

            routeBuilder.MapRoute(
				name: "Products_Product",
				template: "product/{url}",
				defaults: new { controller = "Product", action = "Product" });

            routeBuilder.MapRoute(
                name: "Products_Product_ByIdOld",
                template: "shop/pc/viewPrd.asp",
                defaults: new { controller = "Product", action = "ProductByIdOld" });

            routeBuilder.MapRoute(
				name: "Products_Reviews",
				template: "reviews/{url}/{pageNumber?}",
				defaults: new { controller = "Product", action = "FullReviews" });

			routeBuilder.MapRoute(
                name: "ContentItem_Edit",
                template: "recipe/edit/{id}",
                defaults: new { controller = "Recipe", action = "EditContent" });

            routeBuilder.MapRoute(
                name: "MasterContentItem_Edit",
                template: "recipe/editmaster/{id}",
                defaults: new { controller = "Recipe", action = "EditMasterContent" });

            routeBuilder.MapRoute(
                name: "GoogleProductsFeed",
                template: "feed/datafeed.csv",
                defaults: new { controller = "Help", action = "GoogleProductsFeed" });

            routeBuilder.MapRoute(
                name: "CriteoProductsFeed",
                template: "feed/criteo-datafeed.csv",
                defaults: new { controller = "Help", action = "CriteoProductsFeed" });

            routeBuilder.MapRoute(
                name: "CJProductsFeed",
                template: "feed/cjfeed.csv",
                defaults: new { controller = "Help", action = "CJProductsFeed" });

            routeBuilder.MapRoute(
                name: "CustomCSS",
                template: "custom-styles.css",
                defaults: new { controller = "Help", action = "CustomCSS" });

            routeBuilder.MapRoute(
                name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });
            // Uncomment the following line to add a route for porting Web API 2 controllers.
           //  routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}