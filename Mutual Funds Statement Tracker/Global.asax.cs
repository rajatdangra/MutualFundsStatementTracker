using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
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
            HttpUnhandledException httpUnhandledException =
       new HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError());
            SendEmailWithErrors(httpUnhandledException.GetHtmlErrorMessage());
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

        private static void SendEmailWithErrors(string result)
        {
            try
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("abc", "abc@gmail.com"));
                mailMessage.To.Add(new MailboxAddress("def", "def@gmail.com"));
                mailMessage.Subject = "Mutual Fund Statement Request - Error Occured";
                mailMessage.Body = new TextPart("plain")
                {
                    Text = result
                };

                using (var smtpClient = new SmtpClient())
                {
                    //config settings should be picked from web.config
                    smtpClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtpClient.Authenticate("username", "password");
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Write o the event log.
                logger.Error("Unable to send email.\nException details: " + ex + "\nInner Exception: " + ex.InnerException);
            }
        }
    }
}
