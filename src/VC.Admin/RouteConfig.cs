using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace VC.Admin
{
    public class HomeRouter : IRouter
    {
        private readonly IRouter _defaultHandler;

        public HomeRouter(IRouter defaultHandler)
        {
            _defaultHandler = defaultHandler;
        }

        public Task RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Headers["Accept"].Any(h => h.ToLower().Contains("text/html")))
            {
                return _defaultHandler.RouteAsync(context);
            }
            return TaskCache.CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routeBuilder, IInlineConstraintResolver inlineConstraintResolver)
        {
            routeBuilder.MapRoute(
                name: "defaultApi",
                template: "api/{controller}/{action}/{id?}");

            routeBuilder.MapRoute(
                name: "IEDetect",
                template: "ie",
                defaults: new { controller = "Home", action = "IE" });

            routeBuilder.Routes.Add(new Route(new HomeRouter(routeBuilder.DefaultHandler), "default",
                "{*url}",
                new RouteValueDictionary(new {controller = "Home", action = "Index"}), null, null, inlineConstraintResolver));
        }
    }
}