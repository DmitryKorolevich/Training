using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;

namespace VitalChoice.Public.AppConfig
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
                name: "Recipes_Recipe",
                template: "recipe/{url}",
                defaults: new { controller = "Recipe", action = "Recipe" });

            routeBuilder.MapRoute(
                name: "FAQs_Categories",
                template: "faqs/",
                defaults: new { controller = "FAQ", action = "Categories" });

            routeBuilder.MapRoute(
                name: "FAQs_Category",
                template: "faqs/{url}",
                defaults: new { controller = "FAQ", action = "Category" });

            routeBuilder.MapRoute(
                name: "FAQs_FAQ",
                template: "faq/{url}",
                defaults: new { controller = "FAQ", action = "FAQ" });

            routeBuilder.MapRoute(
                name: "Articles_Categories",
                template: "articles/",
                defaults: new { controller = "Article", action = "Categories" });

            routeBuilder.MapRoute(
                name: "Articles_Category",
                template: "articles/{url}",
                defaults: new { controller = "Article", action = "Category" });

            routeBuilder.MapRoute(
                name: "Articles_Article",
                template: "article/{url}",
                defaults: new { controller = "Article", action = "Article" });

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
                name: "ContentItem_Edit",
                template: "recipe/edit/{id}",
                defaults: new { controller = "Recipe", action = "EditContent" });

            routeBuilder.MapRoute(
                name: "MasterContentItem_Edit",
                template: "recipe/editmaster/{id}",
                defaults: new { controller = "Recipe", action = "EditMasterContent" });

            routeBuilder.MapRoute(
                name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });
            // Uncomment the following line to add a route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}