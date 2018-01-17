using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}, 
                constraints: null,
                handler:  new TokenValidationHandler());
        }
    }
}