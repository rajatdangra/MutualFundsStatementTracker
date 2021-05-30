using MailKit.Net.Smtp;
using MimeKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker
{
    public class Email
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal static string DeveloperEmail = "abc@gmail.com";
        internal static string DeveloperName = "xyz";
        private static string FromEmail = "def@gmail.com";
        private static string FromName = "Mutual Funds Statement Request";

        public static void SendEmail(string message, string subject, string mailIdsTo, string fullNameTo)
        {
            string stInfo = string.Empty;
            try
            {
                logger.Info("SendEmail start.");
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress(FromName, FromEmail));
                foreach (var emailTo in GetEmailIDs(mailIdsTo))
                {
                    mailMessage.To.Add(new MailboxAddress(fullNameTo, emailTo));
                }
                mailMessage.Bcc.Add(new MailboxAddress(DeveloperName, DeveloperEmail));
                mailMessage.Bcc.Add(new MailboxAddress(DeveloperName, FromEmail));

                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart("plain")
                {
                    Text = message
                };

                using (var smtpClient = new SmtpClient())
                {
                    //config settings should be picked from web.config
                    smtpClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(FromEmail, "password");
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                stInfo = "Mail Sent Successfully!";
                //Console.WriteLine(stInfo);
                logger.Info(stInfo);
            }
            catch (Exception ex)
            {
                // Write o the event log.
                stInfo = "Unable to send email.";
                //Console.WriteLine(stInfo);
                logger.Error(stInfo + "\nException details: " + ex + "\nInner Exception: " + ex.InnerException);
            }
        }

        public static List<string> GetEmailIDs(string emailIds)
        {
            return emailIds.Trim().Replace(" ", "").Split(',').ToList();
        }
    }
}