using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Common
{
    public class Logger
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Lưu thông tin lỗi của hệ thống
        /// </summary>
        /// <param name="ex"></param>
        /// <remarks></remarks>
        public static void LogException(Exception ex)
        {
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
        }
    }
}