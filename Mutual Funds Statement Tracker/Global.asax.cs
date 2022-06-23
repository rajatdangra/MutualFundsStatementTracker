using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Mutual_Funds_Statement_Tracker.Models;
using NLog;

namespace Mutual_Funds_Statement_Tracker
{

    public class Global : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.Url.AbsolutePath.EndsWith("/"))
            {
                Server.Transfer(Request.Url.AbsolutePath + "UserDetails.aspx");
            }

            bool forceToHttps = AppConfig.IsForceToHttps;
            //Begin:To force redirect website to https
            if (forceToHttps)
            {
                if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
                {
                    Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"]
                + HttpContext.Current.Request.RawUrl);
                }
            }
            Response.Headers.Remove("Access-Control-Allow-Origin");
            // End: To force redirect website to https
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            HttpUnhandledException httpUnhandledException = new HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError());

            string errorInfo = httpUnhandledException.GetHtmlErrorMessage();
            string detailedErrorInfo = string.Format("An error has been encountered in Mutual Funds Statement Request, Details are mentioned below:\nUser Name: {0}\nEmail: {1}\nPhone: {2}\nPAN: {3}\n\nError Information:\n{4}", AppConfig.FullName, AppConfig.Email, AppConfig.Phone, AppConfig.PAN, errorInfo);
            logger.Error(detailedErrorInfo);
            Console.WriteLine(detailedErrorInfo);

            if (AppConfig.SendEmail)
            {
                var emailObj = new EmailNotifier("Mutual Fund Statement Request - Error Occured", EmailNotifier.DeveloperEmail, EmailNotifier.DeveloperName, isHTMLBody: false);
                emailObj.Notify(detailedErrorInfo);
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }
    }
}
