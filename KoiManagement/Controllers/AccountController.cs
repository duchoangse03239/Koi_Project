using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using WebGrease;
using Member = KoiManagement.Models.Member;
using System.IO;


namespace KoiManagement.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session[SessionAccount.SessionUserId] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(string username, string password)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            try
            {
                //2.1 Input Check
                if (string.IsNullOrWhiteSpace(username))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập tài khoản.";
                    return Json(obj);

                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    obj.Status = 3;
                    obj.Message = "Xin hãy nhập mật khẩu.";
                    return Json(obj);
                }
                // Lấy thông tin user theo Username / Password
                var memberDao = new MemberDAO();
                var user = memberDao.GetMemberByNameAndPass(username, CommonFunction.Md5(password));
                //db.Members.Where(p => p.UserName.Equals(username) && p.Password.Equals(password)).ToList();


                // Check tồn tại
                if (user == null)
                {
                    obj.Status = 4;
                    // khong ton tai
                    obj.Message = "Mật khẩu hoặc tài khoản sai.";
                    return Json(obj);
                }
                else
                {
                    Member member = user;
                    obj.Status = 1;
                    if (member.Status == false)
                    {
                        //User check : Tài khoản đã bị xóa: chuyển màn hình login.
                        obj.Message = "Tài khoản đã bị khóa vui long đăng kí tài khoản mới.";
                        obj.RedirectTo = Url.Action("Login");
                    }
                    else
                    {
                        Session.Add(SessionAccount.SessionUserId, member.MemberID);
                        Session.Add(SessionAccount.SessionUserName, member.UserName);
                        Session.Add(SessionAccount.SessionName, member.Name);
                        Session.Add(SessionAccount.SessionImage, member.Image);
                        SessionAccount.SessionGetUserId = member.MemberID.ToString();
                        obj.RedirectTo = Url.Action("ListVariety", "Variety");

                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }


        public ActionResult Logout()
        {
            try
            {
                Session.Remove(SessionAccount.SessionUserId);
                Session.Remove(SessionAccount.SessionUserName);
                Session.Remove(SessionAccount.SessionName);
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                return RedirectToAction("SystemError", "Error");
            }

            return RedirectToAction("ListVariety", "Variety"); //current man hinh hoac home
        }



        public ActionResult ResetPassword(string actCode)
        {
            if (actCode == null)
            {
                return RedirectToAction("ForgotPassword", "Account");
            }
            ActiveCodeDAO ActiveCodeDAO = new ActiveCodeDAO();
            var isExistActiveCode = ActiveCodeDAO.checkExistCode(actCode);
            if (!isExistActiveCode)
            {
                ViewBag.ErrorActiveCode = "Mã xác nhận đã hết hạn.";
            }
            ViewBag.actCode = actCode;


            return View();
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetPassword(string password, string confirmPassword, string actCode)
        {
            var obj = new StatusObjForJsonResult();

            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    obj.Status = 1;
                    obj.Message = "Mật khẩu mới không được để trống";
                    return Json(obj);
                }
                if (!Validate.CheckLengthInput(password, 6, 32))
                {
                    obj.Status = 1;
                    obj.Message = "Mật khẩu mới phải chứa từ 6 đến 32 ký tự";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    obj.Status = 2;
                    obj.Message = "Xác nhận mật khẩu không được để trống";
                    return Json(obj);
                }
                if (!password.Equals(confirmPassword))
                {
                    obj.Status = 2;
                    obj.Message = "Xác nhận mật khẩu không trùng khớp";
                    return Json(obj);
                }
                // đổi lại pass
                MemberDAO mDao = new MemberDAO();
                ActiveCodeDAO aDao = new ActiveCodeDAO();
                var mem = aDao.GetMemberIdByActCode(actCode);
                if (mem == null)
                {
                    //k tim thay member
                }
                else
                {
                    mDao.ChangePass(mem.MemberID.ToString(), CommonFunction.Md5(password));
                    obj.Status = 3;
                    obj.RedirectTo = Url.Action("Login", "Account");
                    obj.Message = "Mật khẩu của bạn đã được thay đổi";
                    return Json(obj);
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }



        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendEmail(string email)
        {
            var obj = new StatusObjForJsonResult();
            try
            {
                MemberDAO mDao = new MemberDAO();
                ActiveCodeDAO ActCodeDao = new ActiveCodeDAO();
                // Check input email
                if (string.IsNullOrWhiteSpace(email))
                {
                    obj.Status = 1;
                    obj.Message = "Vui lòng nhập địa chỉa email";
                    return Json(obj);
                }

                if (Validate.CheckEmailFormat(email) == false)
                {
                    obj.Status = 2;
                    obj.Message = "Địa chỉ Email không đúng định dạng";
                    return Json(obj);
                }

                // Check exist email 
                var isExistEmail = mDao.CheckExistEmail(email);
                if (isExistEmail)
                {
                    obj.Status = 3;
                    obj.Message = "Địa chỉ email này không tồn tại";
                    return Json(obj);
                }
                else
                {
                    // Get user id
                    var Member = mDao.GetMemberByEmail(email);
                    if (Member != null)
                    {
                        // Auto code
                        String ActRandom = CommonFunction.GenerateString(10);
                        // kiểm tra tồn tại trong database
                        if (!ActCodeDao.checkExistCode(ActRandom))
                        {
                            if (ActCodeDao.AddActiveCode(new ActiveCode(Member.MemberID, ActRandom, DateTime.Now, true)))
                            {
                                var url = CommonFunction.GetScreenUrl(this, "Account", "ResetPassword");
                                if (url.EndsWith("/") == false)
                                {
                                    url += "?actCode=";
                                }
                                url += ActRandom;

                                string content =
                                    System.IO.File.ReadAllText(
                                        Server.MapPath("~/Content/Email/ConfirmEmailWhenForgotPassword.html"));
                                content = content.Replace("{{Link}}", url);
                                CommonFunction.SendMailHelper(email, "Thay đổi mật khẩu tài khoản KoiManagement",
                                    content);
                                obj.Status = 3;
                                obj.Message = "Email đc được gửi, bạn vui long vào mail để kiểm tra";
                            }
                        }
                    }
                    else
                    {
                        obj.Status = 0;
                        obj.RedirectTo = this.Url.Action("SystemError", "Error");
                        return Json(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(string name, string dob, string email, string username, string password,
            string rePassword, string address, string phone)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            MemberDAO dao = new MemberDAO();
            //Khởi tạo giá trị cho trường không bắt buộc
            DateTime? dateOfBirth = new DateTime();
            try
            {
                //2.1 Input Check
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = String.Empty;
                }
                else if (!Validate.CheckLengthInput(name, 6, 50))
                {
                    obj.Status = 1;
                    obj.Message = "Họ tên phải chứa từ 6 đến 50 ký tự";
                    return Json(obj);
                }
                if (Validate.ValidateDate(dob))
                {
                    obj.Status = 2;
                    obj.Message = "Date of birt";
                    return Json(obj);
                }
                else
                {
                    dateOfBirth = Validate.ConverDateTime(dob);
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    obj.Status = 3;
                    obj.Message = "Email không được để trống";
                    return Json(obj);
                }
                if (!Validate.CheckEmailFormat(email))
                {
                    obj.Status = 3;
                    obj.Message = "Email không đúng định dạng";
                    return Json(obj);
                }
                if (!dao.CheckExistEmail(email))
                {
                    obj.Status = 3;
                    obj.Message = "Email đã được sử dụng";
                    return Json(obj);
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    obj.Status = 4;
                    obj.Message = "Tên đăng nhập không được để trống";
                    return Json(obj);
                }

                if (!Validate.CheckEmailFormat(username)&&!Validate.CheckLengthInput(username, 6, 25))
                {
                    obj.Status = 4;
                    obj.Message = "Tên đăng nhập phải chứa từ 6 đến 25 ký tự";
                    return Json(obj);
                }

                    if (!Validate.CheckEmailFormat(username)&&!Validate.CheckNormalCharacter(username))
                    {
                        obj.Status = 4;
                        obj.Message = "Tên đăng nhập không được chứa ký tự đặc biệt";
                        return Json(obj);
                    }

                if (!dao.CheckExistUserName(username))
                {
                    obj.Status = 4;
                    obj.Message = "Tên đăng nhập đã được sử dụng";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    obj.Status = 5;
                    obj.Message = "Mật khẩu không được để trống";
                    return Json(obj);
                }

                if (!Validate.CheckLengthInput(password, 6, 32))
                {
                    obj.Status = 5;
                    obj.Message = "Mật khẩu phải chứa từ 6 dến 32 ký tự";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(rePassword))
                {
                    obj.Status = 6;
                    obj.Message = "Xác nhận mật khẩu không được để trống";
                    return Json(obj);
                }
                if (!Validate.CheckConfirmInput(password, rePassword))
                {
                    obj.Status = 6;
                    obj.Message = "Xác nhận mật khẩu phải trùng với mật khẩu";
                    return Json(obj);
                }

                if (string.IsNullOrWhiteSpace(phone))
                {
                    phone = String.Empty;
                }
                else if (!Validate.CheckLengthInput(phone, 10, 11))
                {
                    obj.Status = 7;
                    obj.Message = "Số điện thoại phải chứa từ 10 đến 11 ký tự";
                    return Json(obj);
                }
                else if (!string.IsNullOrWhiteSpace(phone) && !Validate.CheckIsLong(phone))
                {
                    obj.Status = 7;
                    obj.Message = "Số điện thoại không đúng định dạng";
                    return Json(obj);
                }

                Member me = new Member(name, username, CommonFunction.Md5(password), dateOfBirth, "", "N", phone, email,
                    address, true);
                obj.Status = 9;
                if (dao.AddMember(me))
                {
                    obj.Message = "Đăng ký thành công";
                    obj.RedirectTo = Url.Action("Login", "Account");
                }
                else
                {
                    obj.Status = 8;
                    obj.Message = "Có lỗi xảy ra";
                    return Json(obj);
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

        // GET: /Account/Change Password
        public ActionResult ChangePassword()
        {
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            MemberDAO MDao = new MemberDAO();
            KoiDAO koiDao = new KoiDAO();
            MemberDAO mDAO = new MemberDAO();
            KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            ViewBag.Member = mDAO.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(string oldPassword, string password, string cmfPassword)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                RedirectToAction("Login", "Account");
                return Json(obj);
            }
            MemberDAO dao = new MemberDAO();
            try
            {
                //2.1 Input Check
                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    obj.Status = 1;
                    obj.Message = "Vui lòng nhập mật khẩu cũ";
                    return Json(obj);
                }
                else if (!Validate.CheckLengthInput(oldPassword, 6, 32))
                {
                    obj.Status = 1;
                    obj.Message = "Mật khẩu cũ phải chứa từ 6 đến 32 ký tự";
                    return Json(obj);
                }
                else if (
                    !dao.GetOldPass(Session[SessionAccount.SessionUserId].ToString())
                        .Equals(CommonFunction.Md5(oldPassword)))
                {
                    obj.Status = 1;
                    obj.Message = Common.Message.ChangePasswordM03;
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    obj.Status = 2;
                    obj.Message = "Mật khẩu không được để trống.";
                    return Json(obj);
                }
                else if (!Validate.CheckLengthInput(password, 6, 32))
                {
                    obj.Status = 2;
                    obj.Message = "Mật khẩu phải chứa từ 6 đến 32 ký tự";
                    return Json(obj);
                }
                else if (dao.GetOldPass(Session[SessionAccount.SessionUserId].ToString())
                    .Equals(CommonFunction.Md5(password)))
                {
                    obj.Status = 2;
                    obj.Message = "Mật khẩu cũ phải khác mật khẩu mới";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(cmfPassword))
                {
                    obj.Status = 3;
                    obj.Message = "Xác nhận mật khẩu không được để trống.";
                    return Json(obj);
                }
                else if (!Validate.CheckConfirmInput(password, cmfPassword))
                {
                    obj.Status = 3;
                    obj.Message = "Xác nhận mật khẩu không trùng với mật khẩu.";
                    return Json(obj);
                }

                if (dao.ChangePass(Session[SessionAccount.SessionUserId].ToString(), CommonFunction.Md5(password)) == 1)
                {

                    obj.Status = 0;
                    obj.Message = "Đã đổi mật khẩu thành công";
                    obj.RedirectTo = Url.Action("Login", "Account");
                }
                else
                {
                    obj.Message = "Có lỗi xảy ra";
                    return Json(obj);
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

        // GET: /Account/UpdateProfile
        public ActionResult Profile(int id=0)
        {
            //check ton tai id
            if (id == 0)
            {
                return RedirectToAction("UpdateProfile", "Account");
            }
            MemberDAO MDao = new MemberDAO();
            KoiDAO kDao= new KoiDAO();
            ViewBag.CountKoi = kDao.CountKoibyOwnerId(id);
            var mem = MDao.GetMemberbyID(id);
            return View(mem);
        }

        // GET: /Account/UpdateProfile
        public ActionResult UpdateProfile()
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                RedirectToAction("Login", "Account");
                return Json(obj);
            }
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            MemberDAO MDao = new MemberDAO();
            KoiDAO koiDao = new KoiDAO();
            MemberDAO mDAO = new MemberDAO();
            KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            ViewBag.Member = mDAO.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);

            var mem = MDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            return View(mem);
        }


        // GET: /Account/UpdateProfile
        [HttpPost]
        public JsonResult UpdateProfile(string name, string image, string gender, string dob, string address, string phone, string memberid)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            //Khởi tạo giá trị cho trường không bắt buộc
            DateTime? dateOfBirth = new DateTime();
            try
            {
                //2.1 Input Check
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = String.Empty;
                }
                else if (!Validate.CheckLengthInput(name, 6, 50))
                {
                    obj.Status = 1;
                    obj.Message = "Họ tên phải chứa từ 6 đến 50 ký tự";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(dob)&&Validate.ValidateDate(dob))
                {
                    obj.Status = 2;
                    obj.Message = "Ngày sinh không đúng định dạng";
                    return Json(obj);
                }
                else
                {
                    dateOfBirth = Validate.ConverDateTime(dob);
                }


                if (string.IsNullOrWhiteSpace(phone))
                {
                    phone = String.Empty;
                }
                else if (!Validate.CheckLengthInput(phone, 10, 11))
                {
                    obj.Status = 7;
                    obj.Message = "Số điện thoại phải chứa từ 10 đến 11 ký tự";
                    return Json(obj);
                }
                else if (!string.IsNullOrWhiteSpace(phone) && !Validate.CheckIsLong(phone))
                {
                    obj.Status = 7;
                    obj.Message = "Số điện thoại không đúng định dạng";
                    return Json(obj);
                }

                string filename;
                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có 1 file
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                }

                MemberDAO dao = new MemberDAO();
                Member me = new Member(name, "", "", dateOfBirth, "", gender, phone, "", address, true);
                me.Image = image;
                me.MemberID = int.Parse(memberid);

                //Edit file to local
                if (file != null)
                {
                    if (string.IsNullOrWhiteSpace(me.Image))
                    {
                        filename = Path.GetFileName("mem" + me.MemberID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        me.Image = filename;
                    }
                    else
                    {
                        filename = me.Image;
                    }
                    var fullpath = Server.MapPath("~/Content/Image/Member/" + filename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Member"), filename);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    file.SaveAs(path);
                }

                if (dao.UpdateProfile(me, int.Parse(Session[SessionAccount.SessionUserId].ToString())) == 1)
                {
                    obj.Status = 9;
                    obj.Message = "Cập nhật thành công";
                    obj.RedirectTo = Url.Action("UpdateProfile", "Account");
                    return Json(obj);
                }
                else
                {
                    obj.Status = 8;
                    obj.Message = "Có lỗi xảy ra";
                    return Json(obj);
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }
        [ChildActionOnly]
        public ActionResult Manager(int id)
        {

            // id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            if (id == null&& Session[SessionAccount.SessionUserId]!=null)
            {
                id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            }
                KoiDAO koiDao = new KoiDAO();
                MemberDAO mDAO = new MemberDAO();
                KoiFarmDAO koiFarmDao = new KoiFarmDAO();
                ViewBag.Member = mDAO.GetMemberbyID(id);
                ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
                ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            

            return View("~/Views/Account/_Manager.cshtml");
        }


        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback",
                "Account", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            //  var loginInfo = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            MemberDAO mDao = new MemberDAO();
            var user = mDao.GetMemberByEmail(loginInfo.Email);
            if (user !=null)
            {
                Login(loginInfo.Email, CommonFunction.Md5(loginInfo.Login.ProviderKey));
            }
            else
            {
              //register
                Register("", null,loginInfo.Email, loginInfo.Email, CommonFunction.Md5(loginInfo.Login.ProviderKey), CommonFunction.Md5(loginInfo.Login.ProviderKey),"","");
                Login(loginInfo.Email, CommonFunction.Md5(loginInfo.Login.ProviderKey));
            }
            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() {RedirectUri = RedirectUri};
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}