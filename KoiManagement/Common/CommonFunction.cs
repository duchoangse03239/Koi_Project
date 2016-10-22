using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace KoiManagement.Common
{
    public static class CommonFunction
    {
        static Random rand = new Random();

        private const string Alphabet ="abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        /// <summary>
        /// Lấy base url
        /// </summary>
        /// <returns></returns>
        public static string GetBaseUrl()
        {
            var siteUrl = string.Empty;
            if ((HttpContext.Current != null))
            {
                siteUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            }
            return siteUrl;
        }

        /// <summary>
        /// Lấy Url của 1 screen cụ thể
        /// </summary>
        /// <param name="currentController"></param>
        /// <param name="otherControllerName"></param>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public static string GetScreenUrl(System.Web.Mvc.Controller currentController, string otherControllerName, string screenName)
        {
            var url = GetBaseUrl();
            if (((currentController != null)) && (!string.IsNullOrWhiteSpace(screenName)))
            {
                url += currentController.Url.Action(screenName, otherControllerName);
            }
            return url;
        }

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
    }
}