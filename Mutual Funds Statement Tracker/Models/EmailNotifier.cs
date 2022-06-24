using MailKit.Net.Smtp;
using MimeKit;
using Mutual_Funds_Statement_Tracker.Private;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class EmailNotifier
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal static string DeveloperEmail = PrivateData.DeveloperEmail;
        internal static string DeveloperName = PrivateData.DeveloperName;
        private static string FromEmail = PrivateData.FromEmail;
        private static string FromName = "Mutual Funds Statement Request";
        private static string Password = PrivateData.AppPass/*.MailPass*/;
        
        public EmailNotifier(string subject, string mailIdsTo, string fullNameTo, List<string> files = null, bool isHTMLBody = false)
        {
            Subject = subject;
            MailIdsTo = mailIdsTo;
            FullNameTo = fullNameTo;
            IsHTMLBody = isHTMLBody;
            Files = files ?? new List<string>();
        }

        public string Subject { get; set; }
        public string MailIdsTo { get; set; }
        public string FullNameTo { get; set; }
        public bool IsHTMLBody { get; set; }
        public List<string> Files { get; set; }

        public void Notify(string message)
        {
            string stInfo = string.Empty;
            try
            {
                logger.Info("SendEmail start.");
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress(FromName, FromEmail));
                foreach (var emailTo in GetEmailIDs(MailIdsTo))
                {
                    mailMessage.To.Add(new MailboxAddress(FullNameTo, emailTo));
                }
                mailMessage.Bcc.Add(new MailboxAddress(DeveloperName, DeveloperEmail));
                mailMessage.Bcc.Add(new MailboxAddress(DeveloperName, FromEmail));

                mailMessage.Subject = Subject;

                #region different method for attachment
                //var body = new TextPart(!IsHTMLBody ? "plain" : "html")
                //{
                //    Text = message
                //};
                //mailMessage.Body = body;

                //// create an image attachment for the file located at path
                //var attachment = new MimePart("image", "gif")
                //{
                //    Content = new MimeContent(File.OpenRead(path)),
                //    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                //    ContentTransferEncoding = ContentEncoding.Base64,
                //    FileName = Path.GetFileName(path)
                //};

                //// now create the multipart/mixed container to hold the message text and the
                //// image attachment
                //var multipart = new Multipart("mixed");
                //multipart.Add(body);
                //multipart.Add(attachment);

                //// now set the multipart/mixed as the message body
                //message.Body = multipart;
                #endregion

                var builder = new BodyBuilder();
                if (IsHTMLBody)
                    builder.HtmlBody = message;
                else
                    builder.TextBody = message;

                foreach (var fileName in Files)
                {
                    if (File.Exists(fileName))
                        builder.Attachments.Add(fileName);
                }

                mailMessage.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    //config settings should be picked from web.config
                    smtpClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(FromEmail, Password);
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                stInfo = "Mail Sent Successfully!";
                //ConsoleMethods.PrintSuccess(stInfo);
                logger.Info(stInfo);
            }
            catch (Exception ex)
            {
                // Write o the event log.
                stInfo = "Unable to send email.";
                //ConsoleMethods.PrintError(stInfo);
                logger.Error(stInfo + "\nException details: " + ex + "\nInner Exception: " + ex.InnerException);
            }
        }

        public static List<string> GetEmailIDs(string emailIds)
        {
            return emailIds.Trim().Replace(" ", "").Split(',').ToList();
        }
    }
}