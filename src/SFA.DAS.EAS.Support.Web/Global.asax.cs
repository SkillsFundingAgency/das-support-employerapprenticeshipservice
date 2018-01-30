using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.Authentication;
using SFA.DAS.Support.Shared.SiteConnection;
using SFA.DAS.Web.Policy;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            //throw new Exception();
            //var ioc = DependencyResolver.Current;
            //var logger = ioc.GetService<ILog>();
            //logger.Info("Starting Web Role");

            //MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //var siteConnectorSettings = ioc.GetService<ISiteValidatorSettings>();
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler(siteConnectorSettings, logger));
            //GlobalFilters.Filters.Add(new TokenValidationFilter(siteConnectorSettings, logger));

            //logger.Info("Web role started");
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            //if (HttpContext.Current == null) return;

            //new HttpContextPolicyProvider(
            //    new List<IHttpContextPolicy>()
            //    {
            //        new ResponseHeaderRestrictionPolicy()
            //    }
            //).Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var ex = Server.GetLastError().GetBaseException();
        //    var logger = DependencyResolver.Current.GetService<ILog>();
        //    var logger = new NLogLogger(typeof(HttpApplication));


        //    logger.Error(ex, "App_Error");
        //}
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            BuildAndLogExceptionReport(ex);
        }

        private void BuildAndLogExceptionReport(Exception ex)
        {

            var logger =  new NLogLogger(typeof(HttpApplication));
            ; // DependencyResolver.Current.GetService<ILog>();

            var exceptionReport = $"An Unhandled exception was caught by {nameof(Application_Error)}\r\n";
            exceptionReport += TryAddUserContext();
            exceptionReport += TryAddHttpRequestContext();
            exceptionReport += "\r\nException Stack Trace follows:\r\n\r\n";
            logger.Error(ex, exceptionReport);

        }

        private string TryAddHttpRequestContext()
        {
            try
            {
                return
                    $"Host Address: {HttpContext.Current?.Request?.UserHostAddress ?? "Unknown"}\r\nHttp Method: {HttpContext.Current?.Request?.HttpMethod ?? "Unknown"}\r\nRawUrl: {HttpContext.Current?.Request?.RawUrl ?? "Unknown"}";
            }
            catch (Exception e)
            {
                return "The HttpRequest is not available in this context.";
            }
        }


        private string TryAddUserContext()
        {
            try
            {
                return $"User Name: {HttpContext.Current?.User?.Identity?.Name ?? "Unknown"}\r\n";
            }
            catch (Exception e)
            {
                return "The HttpRequest is not available in this context.";
            }
        }
    }
}