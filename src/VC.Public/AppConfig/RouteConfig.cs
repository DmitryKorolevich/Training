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