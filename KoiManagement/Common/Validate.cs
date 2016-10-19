using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace KoiManagement.Common
{
    public class Validate
    {

        /// <summary>
        /// Check input empty
        /// </summary>
        /// <param name="strinput">string input</param>
        /// <returns>return true if null, false if not null</returns>
        public static bool CheckInputEmptyOrNull(string strinput)
        {
            bool result = string.IsNullOrEmpty(strinput);
            return result;
        }


        /// <summary>
        /// check input's length has deadline
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns>true if invalid input length, other false</returns>
        public static bool CheckLengthInput(string strinput, int minLength, int maxLength)
        {
            bool result = strinput.Length < minLength || strinput.Length > maxLength;
            return result;
        }

        /// <summary>
        /// Check maxlength
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="length"></param>
        /// <returns>true if bigger maxlength,false if smaller than max length</returns>
        public static bool CheckMaxLengthInput(string strinput, int length)
        {
            bool result = strinput.Length > length;
            return result;
        }

        /// <summary>
        /// Check Email format
        /// </summary>
        /// <param name="isValidBeforeCheck">Validation status of the Email before check</param>
        /// <param name="email">Email to check</param>
        /// <param name="isValidAfterCheck">Validation status of the Email after check</param>
        public static void CheckEmailFormat(bool isValidBeforeCheck, string email,  out bool isValidAfterCheck)
        {
            if (isValidBeforeCheck == false)
            {
                isValidAfterCheck = false;
                return;
            }

            var regexFormat = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,17})+)$");
            if (!regexFormat.IsMatch(email.Trim()))
            {
                isValidAfterCheck = false;
            }
            else
            {
                isValidAfterCheck = true;
            }
        }


        /// <summary>
        /// CheckSpecialCharacterInput
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="strregex"></param>
        /// <returns></returns>
        public static bool CheckSpecialCharacterInput(string strinput, string strregex)
        {
            Regex regex = new Regex(@strregex);
            Match match = regex.Match(strinput);
            bool result = match.Success;
            return result;
        }

        /// <summary>
        /// Check Confirm textbox
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="strinput2"></param>
        /// <returns></returns>
        public static bool CheckConfirmInput(string strinput, string strinput2)
        {
            bool result = strinput.Equals(strinput2);
            return result;
        }

        /// <summary>
        /// check valid date
        /// </summary>
        /// <returns></returns>
        public static bool ValidateDate(string stringDateValue)
        {
            try
            {
                CultureInfo cultureInfoDateCulture = new CultureInfo("en-US");
                DateTime d = DateTime.ParseExact(stringDateValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compare 2 DateTime
        /// </summary>
        /// <param name="strDateTo"></param>
        /// <param name="strDateFrom"></param>
        /// <returns></returns>
        public static bool CheckCompareDate(string strDateTo, string strDateFrom)
        {
            if (!ValidateDate(strDateTo) || !ValidateDate(strDateFrom))
            {
                return false;
            }
            if (strDateTo != "" && strDateFrom != "")
            {
                DateTime dateTo = Convert.ToDateTime(strDateTo);
                DateTime dateFrom = Convert.ToDateTime(strDateFrom);

                if (dateTo.Date < dateFrom.Date)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }


    
    }
}