using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web
{
    public class Global : HttpApplication
    {


        void Application_Start(object sender, EventArgs e)
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
            Exception ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<ILog>();
            logger.Error(ex, "App_Error");
        }

    }
}