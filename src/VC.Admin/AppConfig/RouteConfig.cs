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
                template: "content/mastercontentitems",
                defaults: new { controller = "MasterContent", action = "GetMasterContentItems" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "GET" }) });

            routeBuilder.MapRoute(
                name: "MasterContentItem",
                template: "content/mastercontentitems/{id}",
                defaults: new { controller = "MasterContent" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "GET","POST","DELETE" }) });

            routeBuilder.MapRoute(
                name: "ContentTypes",
                template: "content/contenttypes",
                defaults: new { controller = "MasterContent", action = "GetContentTypes" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "GET" }) });

            //routeBuilder.MapRoute(
            //    name: "UpdateGetMasterContentItem",
            //    template: "recipes/{url}",
            //    defaults: new { controller = "MasterContentItem", action = "UpdateMasterContentItem" });

            //routeBuilder.MapRoute(
            //    name: "DeleteMasterContentItem",
            //    template: "recipes/{url}",
            //    defaults: new { controller = "MasterContentItem", action = "UpdateMasterContentItem" });


            routeBuilder.MapRoute(
                name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });
            // Uncomment the following line to add a route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
        }
    }
}