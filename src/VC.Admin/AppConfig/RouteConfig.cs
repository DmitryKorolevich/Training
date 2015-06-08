using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;

namespace VC.Admin.AppConfig
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routeBuilder)
        {
			routeBuilder.MapRoute(
			  name: "defaultApi",
			  template: "api/{controller}/{action}/{id?}");

			routeBuilder.MapRoute(
                name: "default",
                template: "{*url}",
                defaults: new { controller = "Home", action = "Index" });
        }
    }
}