using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class Global : HttpApplication
    {
        //public static Collection<DelegatingHandler> ConfigurationMessageHandlers;

        private void Application_Start(object sender, EventArgs e)
        {
            MvcHandler.DisableMvcResponseHeader = true;
            var ioc = DependencyResolver.Current;
            var logger = ioc.GetService<ILog>();
            logger.Info("Starting Web Role");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler());
        
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