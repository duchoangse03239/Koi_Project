using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace KoiManagement.Common
{
    public class Message
    {
        public const string Loi = "Có lỗi xảy ra.";
        #region Login
        public const string LoginM01 = "Vui lòng nhập tên đăng nhập.";
        public const string LoginM02 = "Vui lòng nhập mật khẩu.";
        public const string LoginM03 = "Tên tài khoản hoặc mật khẩu không đúng.";
        #endregion

        #region Register
        public const string RegisterM01 = "Họ tên phải chứa từ 6 đến 50 ký tự.";
        public const string RegisterM02 = "Không được nhập ngày ở trương lai.";
        public const string RegisterM03 = "Xin nhập đúng định dạng ngày dd/mm/yyyy.";
        public const string RegisterM04 = "Email không được để trống.";
        public const string RegisterM05 = "Email không đúng định dạng.";
        public const string RegisterM06 = "Email đã được sử dụng.";
        public const string RegisterM07 = "Tên đăng nhập không được để trống.";
        public const string RegisterM08 = "Tên đăng nhập không được chứa ký tự đặc biệt";
        public const string RegisterM09 = "Tên đăng nhập phải chứa từ 6 đến 25 ký tự.";
        public const string RegisterM10 = "Tên đăng nhập đã được sử dụng.";
        public const string RegisterM11 = "Mật khẩu không được để trống.";
        public const string RegisterM12 = "Mật khẩu phải chứa từ 6 dến 32 ký tự.";
        public const string RegisterM13 = "Xác nhận mật khẩu không được để trống.";
        public const string RegisterM14 = "Xác nhận mật khẩu phải trùng với mật khẩu.";
        public const string RegisterM15 = "Số điện thoại phải chứa từ 10 đến 11 ký tự.";
        public const string RegisterM16 = "Số điện thoại không đúng định dạng.";
        #endregion

        #region Reset Password
        public const string RePasswordM01 = "Email không được để trống.";
        public const string RePasswordM02 = "Email không hợp lệ.";
        public const string RePasswordM03 = "Không có thành viên nào có email tương tự.";
        #endregion

        #region Change Password
        public const string ChangePasswordM01 = "Mật khẩu cũ không được để trống.";
        public const string ChangePasswordM02 = "Mật khẩu cũ phải chứa từ 6 đến 32 ký tự";
        public const string ChangePasswordM03 = "Mật khẩu cũ không đúng";
        public const string ChangePasswordM04 = "Mật khẩu mới không được để trống.";
        public const string ChangePasswordM05 = "Mật khẩu mới phải chứa từ 6 đến 32 ký tự.";
        public const string ChangePasswordM06 = "Xác nhận mật khẩu không được để trống.";
        public const string ChangePasswordM07 = "Xác nhận mật khẩu phải trùng với mật khẩu.";
        public const string ChangePasswordM08 = "Mật khẩu của bạn đã được thay đổi.";
        #endregion

        #region Update Profile
        public const string UpdateProfileM01 = "Họ và tên phải chứa từ 6 đến 50 ký tự.";
        public const string UpdateProfileM02 = "Không được nhập ngày ở trương lai.";
        public const string UpdateProfileM03 = "Xin nhập đúng định dạng ngày dd/mm/yyyy.";
        public const string UpdateProfileM04 = "Số điện thoại phải chứa từ 10 đến 11 ký tự.";
        public const string UpdateProfileM05 = "Số điện thoại không đúng định dạng.";
        public const string UpdateProfileM06 = "Chỉ được chọn file ảnh.";
        public const string UpdateProfileM07 = "Thông tin của bạn đã được thay đổi.";
        #endregion
    }
}