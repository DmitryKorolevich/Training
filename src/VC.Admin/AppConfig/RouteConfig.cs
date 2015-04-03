using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;

namespace VitalChoice.Admin.AppConfig
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(
                name: "MasterContentItems",
                template: "api/content/mastercontentitems",
                defaults: new { controller = "Content", action = "GetMasterContentItemsAsync" });

            routeBuilder.MapRoute(
                name: "MasterContentItemGet",
                template: "api/content/mastercontentitems/{id}",
                defaults: new { controller = "Content", action = "GetMasterContentItemAsync" });

            routeBuilder.MapRoute(
                name: "MasterContentItemDelete",
                template: "api/content/mastercontentitems/{id}",
                defaults: new { controller = "Content", action = "DeleteMasterContentItemAsync" });

            routeBuilder.MapRoute(
                name: "MasterContentItemUpdate",
                template: "api/content/mastercontentitems/{id?}",
                defaults: new { controller = "Content", action = "UpdateMasterContentItemAsync" });

            routeBuilder.MapRoute(
                name: "ContentTypes",
                template: "api/content/types",
                defaults: new { controller = "Content", action = "GetContentTypesAsync" });

            routeBuilder.MapRoute(
                name: "ContentProcessors",
                template: "api/content/processors",
                defaults: new { controller = "Content", action = "GetContentProcessorsAsync" });

            routeBuilder.MapRoute(
                name: "default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });

            routeBuilder.MapRoute(
               name: "defaultApi",
               template: "api/{controller}/{action}/{id?}");
            // Uncomment the following line to add a route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}