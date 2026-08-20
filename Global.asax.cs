using System;
using System.Web;
using WebAPI.Helpers;

namespace WebAPI
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.LogInfo("Application started.", "Global.Application_Start");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                // Unwrap the ASP.NET wrapper to get the real exception.
                if (ex is HttpUnhandledException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Logger.LogError(ex, "Global.Application_Error (unhandled)");
            }

            // Note: for a JSON API you generally don't want the default
            // ASP.NET error page. Clearing the error here prevents the
            // yellow-screen HTML from leaking. See note below.
            // Server.ClearError();
        }
    }
}