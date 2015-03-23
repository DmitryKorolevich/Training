using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;

namespace VitalChoice.Public
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                name: "Recipes_Categories",
                template: "recipes/categories",
                defaults: new { controller = "Recipe", action = "Categories" });

            routeBuilder.MapRoute(
                name: "Recipes_Category",
                template: "recipes/category/{url}",
                defaults: new { controller = "Recipe", action = "Category" });

            routeBuilder.MapRoute(
                name: "Recipes_Recipe",
                template: "recipes/{url}",
                defaults: new { controller = "Recipe", action = "Recipe" });


            routeBuilder.MapRoute(
                name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });
            // Uncomment the following line to add a route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}