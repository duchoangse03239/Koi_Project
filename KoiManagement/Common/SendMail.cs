using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace KoiManagement.Common
{
    public static class SendMail
    {
        /// <summary>
        /// Send mail helper
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        public static void SendMailHelper(string toEmail, string subject, string content)
        {
            var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"];
            var smtpPost = ConfigurationManager.AppSettings["SMTPPort"];
            MailMessage mail = new MailMessage(fromEmailAddress, toEmail);
            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(smtpPost);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
            client.Host = smtpHost;

            mail.Subject = subject;
            mail.Body = content;
            client.Send(mail);
        }

        //public class SendEmail
        //{
        //    public const string FromEmailAddress = "FromEmailAddress";
        //    public const string FromEmailDisplayName = "FromEmailDisplayName";
        //    public const string FromEmailPassword = "FromEmailPassword";
        //    public const string SmtpHost = "SMTPHost";
        //    public const string SmtpPort = "SMTPPort";
        //    public const string EnabledSsl = "EnabledSSL";
        //}
    }
}