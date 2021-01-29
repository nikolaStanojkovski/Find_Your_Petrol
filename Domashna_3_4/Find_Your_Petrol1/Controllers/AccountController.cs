using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Find_Your_Petrol1.Models;
using System.Collections.Generic;

namespace Find_Your_Petrol1.Controllers
{
    [Authorize]
    ///<summary>
    /// Класата <c>AccountController</c>
    /// се грижи за управување со корисниците во апликацијата
    /// </summary>
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        /// <summary>
        /// Методот <c>AccountController</c>
        /// е дефолтниот конструктор за оваа класа
        /// </summary>
        public AccountController()
        {
        }
        /// <summary>
        /// Методот <c>AccountController</c>
        /// е конструктор со параметри
        /// </summary>
        /// <param name="userManager">Објект од типот ApplicationUserManager</param>
        /// <param name="signInManager">Објект од типот ApplicationSignInManager</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        /// <summary>
        /// Стандарден гетер и стандарден сетер на променливата _signInManager
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }
        /// <summary>
        /// Стандарден гетер и сетер за променливата _userManager
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /// <summary>
        /// Методот <c>Login</c>
        /// се справува со GET барањата на патека /Account/Login
        /// </summary>
        /// <param name="returnUrl">Аргумент од типот String, што ни означува повратен URL</param>
        /// <returns>
        /// Враќа поглед
        /// </returns>
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Методот <c>Login</c>
        /// се справува со POST барањата на патека /Account/Login
        /// и притоа врши валидација на најавениот корисник
        /// </summary>
        /// <param name="model">Објект од типот LoginViewModel</param>
        /// <param name="returnUrl">Аргумент од типот String, што ни означува повратен URL</param>
        /// <returns>
        /// Во зависност од тоа дали е успешна најавата, враќа соодветен поглед
        /// </returns>
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        /// <summary>
        /// Методот <c>VerifyCode</c>
        /// се справува со GET барањата на патека /Account/VerifyCode
        /// </summary>
        /// <param name="provider">Променлива од типот string</param>
        /// <param name="returnUrl">Променлива од типот string</param>
        /// <param name="rememberMe">Променлива од типот bool</param>
        /// <returns>
        /// Во зависност од тоа дали корисникот е најавен, 
        /// се враќа соодветен поглед
        /// </returns>
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// Методот <c>VerifyCode</c>
        /// се справува со POST барањата на патека /Account/VerifyCode
        /// </summary>
        /// <param name="model">Објект од типот VerifyCodeViewModel</param>
        /// <returns>
        /// Во зависност од тоа дали корисникот е најавен, 
        /// се враќа соодветен поглед
        /// </returns>
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        /// <summary>
        /// Методот <c>Register</c>
        /// се справува со GET барањата на патека /Account/Register
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>Register</c>
        /// се справува со POST барањата на патека /Account/Register
        /// и регистрира нов корисник во апликацијата
        /// </summary>
        /// <param name="model">Објект од типот RegisterViewModel</param>
        /// <returns>
        /// Враќа соодветен поглед во зависност дали корисникот се регистрирал успешно
        /// </returns>
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }

        /// <summary>
        /// Методот <c>ConfirmEmail</c>
        /// се справува со GET барањата на патека /Account/ConfirmEmail
        /// и му овозможува на корисникот да го потврдни својот е-маил
        /// </summary>
        /// <param name="userId">Променлива од типот string, што претставува Id на корисникот</param>
        /// <param name="code">Променлива од типот string, што претставува код за корисникот</param>
        /// <returns></returns>
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Методот <c>ForgotPassword</c>
        /// се справува со GET барањата на патека /Account/ForgotPassword
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>ForgotPassword</c>
        /// се справува со POST барањата на патека /Account/ForgotPassword
        /// и му овозможува на корисникот да ја промени својата лозинка
        /// </summary>
        /// <param name="model">Објект од типот ForgotPasswordViewModel</param>
        /// <returns>
        /// Враќа соодветен поглед, во зависност дали поминало успешно внесувањето на нова лозинка
        /// </returns>
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }

            }
            return View(model);
        }

        /// <summary>
        /// Методот <c>ForgotPasswordConfirmation</c>
        /// се справува со GET барањата на патека /Account/ForgotPasswordConfirmation
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>ResetPassword</c>
        /// се справува со GET барањата на патека /Account/ResetPassword
        /// и му овозможува враќање на корисникот поглед за соодветна промена на лозинката
        /// </summary>
        /// <param name="code">Променлива од типот string</param>
        /// <returns>
        /// Враќа соодветен поглед доколку влезниот параметар е null, ќе врати поглед со порака Error,
        /// во спротивно ќе врати само поглед
        /// </returns>
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// Методот <c>ResetPassword</c>
        /// се справува со POST барањата на патека /Account/ResetPassword
        /// и му овозможува на корисникот да ја промени својата лозинка
        /// </summary>
        /// <param name="model">Објект од типот ResetPasswordViewModel</param>
        /// <returns>
        /// Доколку влезниот параметар е валиден, ќе врати редирекција кон /Account/ResetPasswordConfirmation
        /// во спротивно ќе го врати истиот поглед
        /// </returns>
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Методот <c>ResetPasswordConfirmation</c>
        /// се справува со GET барањата на патека /Account/ResetPasswordConfirmation
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>ExternalLogin</c>
        /// се справува со POST барањата на патека /Account/ExternalLogin
        /// и му овозможува на корисникот да се најави со некој надворешен провајдер
        /// </summary>
        /// <param name="provider">Променлива од типот string, претставува провајдер</param>
        /// <param name="returnUrl">Променлива од типот string, што ни означува патека на која ќе се редиректира при успешно извршена работа</param>
        /// <returns></returns>
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// Методот <c>SendCode</c>
        /// се справува со GET барањата на патека /Account/SendCode
        /// </summary>
        /// <param name="returnUrl">Променлива од типот string, што ни означува патека на која ќе се редиректира при успешно извршена работа</param>
        /// <param name="rememberMe">Променлива од типот bool, што означува дали корисникот да остане најавен и после исклучувањето на апликацијата</param>
        /// <returns>
        /// Враќа соодветен поглед, доколку корисникот е најавен враќа поглед со порака Error
        /// </returns>
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// Методот <c>SendCode</c>
        /// се справува со POST барањата на патека /Account/SendCode
        /// </summary>
        /// <param name="model">Објект од типот SendCodeViewModel</param>
        /// <returns>
        /// Враќа соодветен поглед доколку настанала грешка или редирекртира кон /Account/VerifyCode
        /// </returns>
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        /// <summary>
        /// Методот <c>ExternalLoginCallback</c>
        /// се справува со GET барањата на патека /Account/ExternalLoginCallback
        /// и му овозможува на корисникот да се најави преку надворешен провајдер
        /// </summary>
        /// <param name="returnUrl">Променлива од типот string, што ни означува патека на која ќе се редиректира при успешно извршена работа</param>
        /// <returns>
        /// Доколку корисникот не е најавен на апликација враќа редирекција кон /Account/Login,
        /// во спротивно враќа соодветен поглед
        /// </returns>
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        /// <summary>
        /// Методот <c>ExternalLoginConfirmation</c>
        /// се справува со POST барањата на патека /Account/ExternalLoginConfirmation
        /// и му овозможува на корисникот којшто е најавен преку надворешен провајдер да ја потврди својата најава
        /// </summary>
        /// <param name="model">Објект од типот ExternalLoginConfirmationViewModel</param>
        /// <param name="returnUrl">Променлива од типот string, што ни означува патека на која ќе се редиректира при успешно извршена работа</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// Методот <c>LogOff</c>
        /// се спавува со POST барањата на патека /Account/LogOff
        /// и му овозможува на корисникот да се одјави од апликацијата
        /// </summary>
        /// <returns>
        /// Враќа редирекција кон /Home/Index
        /// </returns>
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Методот <c>ExternalLoginFailure</c>
        /// се справува со GET барањата на патека /Account/ExternalLoginFailure
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>AddUserToRole</c>
        /// се справува со GET барањата на патека /Account/AddUserToRole
        /// и му овозможува на корисникот кој е во улога “Administrator“
        /// да додели улога на некој друг корисник
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Account/AddUserToRole
        [Authorize(Roles = "Administrator")]
        public ActionResult AddUserToRole()
        {
            AddRoleToUsers model = new AddRoleToUsers();
            model.Roles.Add("Administrator");
            model.Roles.Add("User");
            ViewBag.UserName = this.User.Identity.Name;

            var AllUsers = UserManager.Users.ToList();
            var WithoutCurrentUser = new List<ApplicationUser>();

            foreach(var item in AllUsers)
            {
                if(item.UserName != this.User.Identity.Name)
                {
                    WithoutCurrentUser.Add(item);
                }
            }

            ViewBag.Users = WithoutCurrentUser;
            return View(model);
        }

        /// <summary>
        /// Методот <c>AddUserToRole</c>
        /// се спраува со POST барањата на патека /Account/AddUserToRole
        /// и му овозможува на корисникот кој е во улога “Administrator“
        /// да додели улога на некој друг корисник
        /// </summary>
        /// <param name="model">Објект од типот AddRoleToUsers</param>
        /// <returns>
        /// Враќа редирекција кон /Home/Index при успешно завршена работа,
        /// во спротивно враќа објект од типот HttpNotFound
        /// </returns>
        [HttpPost]
        public ActionResult AddUserToRole(AddRoleToUsers model)
        {
            try
            {
                var user = UserManager.FindByEmail(model.SelectedUserEmail);
                UserManager.RemoveFromRole(user.Id, "Administrator");
                UserManager.RemoveFromRole(user.Id, "User");
                UserManager.AddToRole(user.Id, model.SelectedRole);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception e)
            {
                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
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