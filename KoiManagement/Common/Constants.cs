using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoiManagement.Hub;

namespace KoiManagement.Common
{
    public static class Constants
    {
        public const string BackLinkTitleToHome = "Back to Home";
        public const string BackLinkTitleToLogin = "Back to Login";
        public static List<UserConnection> _uList = new List<UserConnection>();

        public const string EmailBodyResetPass ="<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<title> Thay đổi mật khẩu tài khoản Dịch vụ quản lý cá koi </title>\r\n<meta charset=\"utf-8\" />\r\n " +
            "</head>\r\n<body>\r\n<br/>\r\nVui lòng click vào đường link dưới đây để thay đổi mật khẩu: <br/>\r\n" +
            "<a href=\"{{Link}}\">{{Link}}</a>\r\n <br/>\r\n<br/>\r\n<label class=\"text-danger\">" +
            "Nếu bạn không click vào đường link trên trong vào 30 phút thì yêu cầu thay đổi mật khẩu sẽ mất hiệu lực!</label>\r\n </body>\r\n </html>";
        
        
        public const string EmailTitleResetPass = "Đặt lại mật khẩu tài khoản KSMS";
        public const string AdminID = "1";

    }
}