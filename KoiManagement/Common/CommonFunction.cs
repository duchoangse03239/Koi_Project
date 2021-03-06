﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

        /// Mã hóa MD5
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncryptData(string data)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedBytes = null;
            var encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }

        public static string Md5(string data)
        {
            return BitConverter.ToString(EncryptData(data)).Replace("-", "").ToLower();
        }


        /// <summary>
        /// Covert time client to time server
        /// </summary>
        /// <param name="dateTime">DateTime client</param>
        /// <returns>DateTime server</returns>
        public static DateTime ConvertToDateTime(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                return new DateTime();
            }
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTime dateReturn = DateTime.ParseExact(dateTime, "yyyy-MM-dd", culture);
            return dateReturn;
        }

        public static string convertToNormalString(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Ép kiểu int 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int? ToNullableInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        /// <summary>
        /// Ép kiểu int 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal? ToNullableDecimal(this string s)
        {
            decimal i;
            if (decimal.TryParse(s, out i)) return i;
            return null;
        }

        public static string GetTimeInterval(DateTimeOffset createdDate)
        {
            DateTime start = DateTime.Now;
            // Do some work
            TimeSpan timeDiff = DateTime.Now - createdDate;
            string result;
            if (timeDiff.TotalDays >= 30)
            {
                //Tailm :delete "Tu"
                result = createdDate.Date.ToShortDateString();
            }
            else if (timeDiff.TotalHours >= 24)
            {
                result = Math.Round(timeDiff.TotalDays) + " ngày trước ";
            }
            else if (timeDiff.TotalMinutes >= 60)
            {
                result = Math.Round(timeDiff.TotalHours) + " giờ trước ";
            }
            else if (timeDiff.TotalSeconds >= 60)
            {
                result = Math.Round(timeDiff.TotalMinutes) + " phút trước";
            }
            else
            {
                result = " Vừa xong ";
            }
            return result;
        }
        public static string Trim2LastCharacter(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.TrimEnd().Substring(0, str.Length - 2);
            }
        }

        public static string ConvertGender(string gender)
        {
            if (String.IsNullOrEmpty(gender))
            {
                return "N";
            }

            if (gender.Equals("1"))
            {
                return "M";
            }
            else if (gender.Equals("1"))
            {
                return "F";
            }
            return "N";
        }

        public static string ReturnGenderKoi(string gender)
        {
            if (!string.IsNullOrWhiteSpace(gender)) { 
            if (gender.Equals("M"))
            {
                return "Đực";
            }
            else if (gender.Equals("F"))
            {
                return "Cái";
            }
            }
                return "Chưa rõ";
        }
        public static string ReturnGenderKoiDb(string gender)
        {
            if (gender.Equals("Đực"))
            {
                return "M";
            }
            else if (gender.Equals("Cái"))
            {
                return "F";
            }
            else
            {
                return "N";
            }
        }

        public static string ReturnGenderMem(string gender)
        {
            if (!string.IsNullOrWhiteSpace(gender))
            {
                if (gender.Equals("M"))
            {
                return "Nam";
            }
            else if (gender.Equals("F"))
            {
                return "Nữ";
            }
            }
            return "Chưa rõ";

        }

        public static DateTime GetAgeMonth(DateTime? birthday)
        {
            DateTime now = DateTime.Today;
            //int Month = 0;
            //Month = 12 * (now.Year - birthday.Value.Year) + (now.Month - birthday.Value.Month);
            //DateTime a;
            //var subYear = now.Year - Month/12;
            //a = now.AddYears(-(now.Year-subYear));
            return now;


        }
        // compute difference in total months
        public static DateTime? getDateFromMonth(int months)
        {
            DateTime now = DateTime.Today;
            DateTime? t;
            if (months < 0) return null;
            var month1 = now.Month - months % 12;
            if (month1 == 0) month1 = 1;
            var year = now.Year - months / 12;
            if (month1 < -0)
            {
                month1 = month1 + 12;
                year = year - 1;
            }
            string date = "01/" + month1 + "/" + year;
            date = Validate.ConverDateExcel(date);
            t = Validate.ConverDateTime(date);
            return t;
        }

        public static int getindex(string origin, string text)
        {
            var s1 = origin.ToCharArray();
            var s2 = text.ToCharArray();
            for (int i = 0; i < s1.Length; i++)
            {
                if (s2.Length > i)
                {
                    if (!s1[i].Equals(s2[i]))
                        return s1.Length - i;
                }
                else
                {
                    break;
                }
            }
            return s2.Length;
        }

        public static string calculateAge(DateTime birthDate)
        {
            birthDate = birthDate.Date;
            //var now = DateTime.Now;

            // var days = now.Day - birthDate.Day;
            // if (days < 0)
            // {
            //     var newNow = now.AddMonths(-1);
            //     days += (int)(now - newNow).TotalDays;
            //     now = newNow;
            // }
            // var months = now.Month - birthDate.Month;
            // if (months < 0)
            // {
            //     months += 12;
            //     now = now.AddYears(-1);
            // }
            // var years = now.Year - birthDate.Year;
            // if (years == 0)
            // {
            //     if (months == 0)
            //         return days.ToString() + " ngày";
            //     else
            //         return months.ToString() + " tháng";
            // }
            // return years.ToString() + " Tuổi";

            DateTime bd = birthDate;
            TimeSpan ts = DateTime.Now.Subtract(bd);
            DateTime age = DateTime.MinValue + ts;
            string s = string.Format("{0} năm {1} tháng tuổi ", age.Year - 1, age.Month - 1);
            return s;
        }

        public static DateTime GetNowDate(DateTime aDateTime)
        {
            return DateTime.Now;
        }

    }
}