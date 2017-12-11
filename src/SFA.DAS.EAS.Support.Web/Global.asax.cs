using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            var logger = DependencyResolver.Current.GetService<ILog>();
            logger.Info("Starting Web Role");

            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            logger.Info("Web role started");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<ILog>();
            logger.Error(ex, "App_Error");
        }
    }
}