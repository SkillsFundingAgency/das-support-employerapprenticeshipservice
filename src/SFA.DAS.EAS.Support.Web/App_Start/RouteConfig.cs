using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "id",
                "{controller}/{id}",
                new {controller = "Home", action = "Index"}
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}