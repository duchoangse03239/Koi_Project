using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.Models;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Model.DAO;
using WebGrease;


namespace KoiManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public JsonResult CheckLogin(string username, string password )
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
                var user = memberDao.GetMemberByNameAndPass(username, password);
                    //db.Members.Where(p => p.UserName.Equals(username) && p.Password.Equals(password)).ToList();
                

                // Check tồn tại
                if (user.Count==0)
                {
                    obj.Status = 4;
                    // khong ton tai
                    obj.Message = "Mật khẩu hoặc tài khoản sai.";
                    return Json(obj);
                }
                else
                {
                    Member member = user.First();
                    obj.Status = 1;
                    if (member.Status==false)
                    {
                        //User check : Tài khoản đã bị xóa: chuyển màn hình login.
                        obj.Message = "Tài khoản đã bị khóa vui long đăng kí tài khoản mới.";
                        obj.RedirectTo = Url.Action("Login");
                    }
                    else
                    {
                        Session.Add(SessionAccount.SessionUserId,member.MemberID);
                        Session.Add(SessionAccount.SessionUserName, member.UserName);
                        Session.Add(SessionAccount.SessionName, member.Name);
                        obj.RedirectTo = Url.Action("ListVariety","Variety");

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                obj.RedirectTo = Url.Action("SystemError","Error");
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
                _logger.Error(ex.Message);
                return RedirectToAction("SystemError", "Error");
            }

            return RedirectToAction("ListVariety", "Variety"); //current man hinh hoac home
        }



        public ActionResult Resetpass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SentEmail(string Email)
        {
            Common.SendMail.SendMailHelper(Email, Constants.EmailTitleResetPass, Constants.EmailBodyResetPass);
            return View();
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
                    obj.Status =2;
                    obj.Message = "Địa chỉ Email không đúng định dạng";
                    return Json(obj);
                }

                // Check exist email 
                var isExistEmail = mDao.CheckExistEmail(email);
                if (isExistEmail )
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
                           // if (ActCodeDao.AddActiveCode(new ActiveCode(Member.MemberID, ActRandom, DateTime.Now, true)))
                            {
                                var url = CommonFunction.GetScreenUrl(this, "Account","ResetPassword");
                                if (url.EndsWith("/") == false)
                                {
                                    url += "?actCode=";
                                }
                                url += ActRandom;

                                string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Email/ConfirmEmailWhenForgotPassword.html"));
                                content = content.Replace("{{Link}}", url);
                                CommonFunction.SendMailHelper(email, "Thay đổi mật khẩu tài khoản KoiManagement",content);
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
                _logger.Error(ex.Message);
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
        public JsonResult Register(string name, string dob, string email, string username, string password, string rePassword, string address, string phone)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            MemberDAO dao = new MemberDAO();
            //Khởi tạo giá trị cho trường không bắt buộc
            DateTime? dateOfBirth = null;
            try
            {
                //2.1 Input Check
                if (string.IsNullOrWhiteSpace(username))
                {
                    obj.Status = 1;
                    obj.Message = "Tên đăng nhập không được để trống";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    obj.Status = 2;
                    obj.Message = "Mật khẩu không được để trống";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(rePassword))
                {
                    obj.Status = 3;
                    obj.Message = "Xác nhận mật khẩu không được để trống";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    obj.Status = 5;
                    obj.Message = "Email không được để trống";
                    return Json(obj);
                }
                if (!Validate.CheckLengthInput(username, 6, 25))
                {
                    obj.Status = 6;
                    obj.Message = "Tên đăng nhập phải chứa từ 6 đến 25 ký tự";
                    return Json(obj);
                }
                if (!Validate.CheckNormalCharacter(username))
                {
                    obj.Status = 7;
                    obj.Message = "Tên đăng nhập không được chứa ký tự đặc biệt";
                    return Json(obj);
                }
                if (!dao.CheckExistUserName(username))
                {
                    obj.Status = 8;
                    obj.Message = "Tên đăng nhập đã được sử dụng";
                    return Json(obj);
                }
                if (!Validate.CheckLengthInput(password, 6, 50))
                {
                    obj.Status = 9;
                    obj.Message = "Mật khẩu phải chứa từ 6 dến 32 ký tự";
                    return Json(obj);
                }
                if (!Validate.CheckConfirmInput(password, rePassword))
                {
                    obj.Status = 10;
                    obj.Message = "Xác nhận mật khẩu phải trùng với mật khẩu";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = String.Empty;
                }
                else if (!Validate.CheckLengthInput(name, 6, 50))
                {
                    obj.Status = 11;
                    obj.Message = "Họ tên phải chứa từ 6 đến 50 ký tự";
                    return Json(obj);
                }

                if (Validate.ValidateDate(dob))
                {
                    obj.Status = 12;
                    obj.Message = "Date of birt";
                    dateOfBirth = Validate.ConverDateTime(dob);
                    return Json(obj);
                }

                if (!Validate.CheckEmailFormat(email))
                {
                    obj.Status = 13;
                    obj.Message = "Email không đúng định dạng";
                    return Json(obj);
                }
                if (!dao.CheckExistEmail(email))
                {
                    obj.Status = 14;
                    obj.Message = "Email đã được sử dụng";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(phone))
                {
                    phone = String.Empty;
                }
                else if (!Validate.CheckLengthInput(phone, 10, 11))
                {
                    obj.Status = 15;
                    obj.Message = "Số điện thoại phải chứa từ 10 đến 11 ký tự";
                    return Json(obj);
                }
                else if (!string.IsNullOrWhiteSpace(phone) && !Validate.CheckIsLong(phone))
                {
                    obj.Status = 16;
                    obj.Message = "Số điện thoại không đúng định dạng";
                    return Json(obj);
                }

                Member me = new Member(name, username, password, dateOfBirth, "", "N", phone, email, "", true);
                obj.Status = 17;
                if (dao.AddMember(me))
                {
                    obj.Message = "Đăng ký thành công";
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
                _logger.Error(ex.Message);
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
            }
            return Json(obj);
        }

        // GET: /Account/Change Password
        public ActionResult ChangePassword()
        {
            return View();
        }

        
        public JsonResult ChangePassword(string olPassword, string password, string cmfPassword)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            MemberDAO dao = new MemberDAO();
            try
            {
                ////2.1 Input Check
                //if (string.IsNullOrWhiteSpace(username))
                //{
                //    obj.Status = 1;
                //    obj.Message = "Tên đăng nhập không được để trống";
                //    return Json(obj);
                //}
                //if (string.IsNullOrWhiteSpace(password))
                //{
                //    obj.Status = 2;
                //    obj.Message = "Mật khẩu không được để trống";
                //    return Json(obj);
                //}
                //if (string.IsNullOrWhiteSpace(rePassword))
                //{
                //    obj.Status = 3;
                //    obj.Message = "Xác nhận mật khẩu không được để trống";
                //    return Json(obj);
                //}
                //if (string.IsNullOrWhiteSpace(email))
                //{
                //    obj.Status = 5;
                //    obj.Message = "Email không được để trống";
                //    return Json(obj);
                //}
                //if (!Validate.CheckLengthInput(username, 6, 25))
                //{
                //    obj.Status = 6;
                //    obj.Message = "Tên đăng nhập phải chứa từ 6 đến 25 ký tự";
                //    return Json(obj);
                //}
                //if (!Validate.CheckNormalCharacter(username))
                //{
                //    obj.Status = 7;
                //    obj.Message = "Tên đăng nhập không được chứa ký tự đặc biệt";
                //    return Json(obj);
                //}
                //if (!dao.CheckExistUserName(username))
                //{
                //    obj.Status = 8;
                //    obj.Message = "Tên đăng nhập đã được sử dụng";
                //    return Json(obj);
                //}

                if (!dao.GetOldPass(Session[SessionAccount.SessionUserId].ToString()).Equals(olPassword))
                {
                    obj.Status = 3;
                    obj.Message = Common.Message.ChangePasswordM03;
                }

                if (dao.GetOldPass(Session[SessionAccount.SessionUserId].ToString()).Equals(password))
                {
                    obj.Message = Common.Message.ChangePasswordM06;
                }

                if (dao.ChangePass(Session[SessionAccount.SessionUserId].ToString(), password) == 1)
                {
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
                _logger.Error(ex.Message);
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
            }
            return Json(obj);
        }



        //public ActionResult Edit()
        //{
        //    var model = db.Members.Find(User.Identity.Name);
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Edit(Member model)
        //{
        //    var f = Request.Files["upPhoto"];
        //    if (f != null && f.ContentLength > 0)
        //    {
        //        var ext = f.FileName.Substring(f.FileName.LastIndexOf("."));
        //        var path = Server.MapPath("/Content/img/customers/" + model.UserName + ext);
        //        f.SaveAs(path);
        //     //   model.Photo = model.Id + ext;
        //    }
        //    db.Entry(model).State = EntityState.Modified;
        //    db.SaveChanges();

        //    return View(model);
        //}

        //[AllowAnonymous]
        //public ActionResult Activate(String UserName)
        //{
        //    var user = db.Members.Find(UserName);
        //    user.Status = true;
        //    db.SaveChanges();
        //    return RedirectToAction("Login");
        //}

        //[AllowAnonymous]
        //public ActionResult Forgot()
        //{
        //    return View();
        //}
        //[AllowAnonymous, HttpPost]
        //public ActionResult Forgot(String UserName, String Email)
        //{
        //    try
        //    {
        //        var cust = db.Members.Single(c => c.UserName == UserName && c.Email == Email);

        //        var user = UserManager.FindByName(UserName);

        //        String TokenCode = Guid.NewGuid().ToString();
        //        UserManager.RemovePassword(user.Id);
        //        UserManager.AddPassword(user.Id, TokenCode);
        //        //XMail.Send(Email, "Token Code", TokenCode);

        //        return View("Reset");
        //    }
        //    catch
        //    {
        //        ModelState.AddModelError("", "Sai thông tin user !");
        //        return View();
        //    }
        //}
        //[AllowAnonymous, HttpPost]
        //public ActionResult Reset(String UserName, String TokenCode, String NewPassword)
        //{
        //    var user = UserManager.FindByName(UserName);
        //    UserManager.ChangePassword(user.Id, TokenCode, NewPassword);
        //    return RedirectToAction("Login");
        //}





        ////
        //// POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    ViewBag.HasLocalPassword = HasPassword();
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        ////
        //// POST: /Account/Manage
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = HasPassword();
        //    ViewBag.HasLocalPassword = hasPassword;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        //    }
        //}

        ////
        //// POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //}

        ////
        //// GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser()
        //        {
        //            UserName = model.UserName,
        //            Email = model.Email,
        //            BirthDate = model.BirthDate
        //        };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInAsync(user, isPersistent: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// GET: /Account/LogOff
        //public ActionResult LogOff()
        //{
        //    AuthenticationManager.SignOut();
        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && UserManager != null)
        //    {
        //        UserManager.Dispose();
        //        UserManager = null;
        //    }
        //    base.Dispose(disposing);
        //}





        //#region Helpers
        //// Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private bool HasPassword()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PasswordHash != null;
        //    }
        //    return false;
        //}

        //public enum ManageMessageId
        //{
        //    ChangePasswordSuccess,
        //    SetPasswordSuccess,
        //    RemoveLoginSuccess,
        //    Error
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //private class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        //#endregion
    }
}