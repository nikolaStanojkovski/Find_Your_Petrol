using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Find_Your_Petrol1.Models;

namespace Find_Your_Petrol1.Controllers
{
    [Authorize]
    ///<summary>
    ///Класата <c>ManageController</c>
    ///се справува со корисниците и нивната состојба во нашата апликација
    /// </summary>
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        /// <summary>
        /// Методот <c>ManageController</c>
        /// е дефолтниот конструктор за оваа класа
        /// </summary>
        public ManageController()
        {
        }
        /// <summary>
        /// Методот <c>ManageController</c>
        /// е конструктор со параметри
        /// </summary>
        /// <param name="userManager">Објект од типот ApplicationUserManager</param>
        /// <param name="signInManager">Објект од типот ApplicationSignInManager</param>
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        /// Методот <c>Index</c>
        /// се справува со GET барањата на патека /Manage/Index
        /// </summary>
        /// <param name="message">Овјект од типот ManageMessageId</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Вашата лозинка е променета."
                : message == ManageMessageId.SetPasswordSuccess ? "Вашата лозинка е сетирана."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Вашиот two-factor authentication provider е сетиран."
                : message == ManageMessageId.Error ? "Се појави грешка."
                : message == ManageMessageId.AddPhoneSuccess ? "Вашиот телефонски број е додаден."
                : message == ManageMessageId.RemovePhoneSuccess ? "Вашиот телефонски број е избришан."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        /// <summary>
        /// Методот <c>RemoveLogin</c>
        /// се справува со POST барањата на патека /Manage/RemoveLogin
        /// </summary>
        /// <param name="loginProvider">Аргумент од типот string</param>
        /// <param name="providerKey">Аргумент од типот string</param>
        /// <returns>
        /// Враќа редирекција кон /Manage/ManageLogins
        /// </returns>
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        /// <summary>
        /// Методот <c>AddPhoneNumber</c>
        /// се справува со GET барањата на патека /Manage/AddPhoneNumber
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>AddPhoneNumber</c>
        /// се спавува со POST барањата на патека /Manage/AddPhoneNumber
        /// и овозможува додавање на телефонски број на корисник
        /// </summary>
        /// <param name="model">Објект од типот AddPhoneNumberViewModel</param>
        /// <returns>
        /// Враќа редирекција кон /Manage/VerifyPhoneNumber доколку влезниот објект е валиден,
        /// во спротивно го враќа истиот поглед од GET барањето
        /// </returns>
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        /// <summary>
        /// Методот <c>EnableTwoFactorAuthentication</c>
        /// се справува со POST барањата на патека /Manage/EnableTwoFactorAuthentication
        /// и му овозможува на корисникот да вклучи TwoFactorAuthentication
        /// </summary>
        /// <returns>
        /// Враќа редирекција кон /Manage/Index
        /// </returns>
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// Методот <c>DisableTwoFactorAuthentication</c>
        /// се справува со POST барањата на патека /Manage/DisableTwoFactorAuthentication
        /// и му овозможува на корисникот да го исклучи TwoFactorAuthentication
        /// </summary>
        /// <returns>
        /// Враќа редирекција кон /Manage/Index
        /// </returns>
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// Методот <c>VerifyPhoneNumber</c>
        /// се справува со GET барањата на патека /Manage/VerifyPhoneNumber
        /// </summary>
        /// <param name="phoneNumber">Аргумент од тип string, што претставува телефонски број</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);

            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        /// <summary>
        /// Методот <c>VerifyPhoneNumber</c>
        /// се справува со POST барањата на патека /Manage/VerifyPhoneNumber
        /// и му овозможува на корисникот да го верификува својот телефонскиот број
        /// </summary>
        /// <param name="model">Објект од типот VerifyPhoneNumberViewModel</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        /// <summary>
        /// Методот <c>RemovePhoneNumber</c>
        /// се справува со POST барањата на патека /Manage/RemovePhoneNumber
        /// и му овозможува на корисникот да го избрише својот телефонски број
        /// </summary>
        /// <returns>
        /// Враќа редирекција кон /Manage/Index
        /// </returns>
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        /// <summary>
        /// Методот <c>ChangePassword</c>
        /// се справува со GET барањата на патека /Manage/ChangePassword
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>ChangePassword</c>
        /// се справува со POST барањата на патека /Manage/ChangePassword
        /// и му овозможува на корисникот да ја промени својата лозинка
        /// </summary>
        /// <param name="model">Објект од типот ChangePasswordViewModel</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        /// <summary>
        /// Методот <c>SetPassword</c>
        /// се спавува со GET барањата на патека /Manage/SetPassword
        /// </summary>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        /// <summary>
        /// Методот <c>SetPassword</c>
        /// се справува со POST барањата на патека /Manage/SetPassword
        /// и му овозможува на корисникот да ја сетира својата лозинка
        /// </summary>
        /// <param name="model">Објект од типот SetPasswordViewModel</param>
        /// <returns>
        /// Враќа редирекција кон /Manage/Index, доколку се извршило успешно работата
        /// </returns>
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Методот <c>ManageLogins</c>
        /// се справува со GET барањата на патека /Manage/ManageLogins
        /// </summary>
        /// <param name="message">Објект од типот ManageMessageId</param>
        /// <returns>
        /// Враќа соодветен поглед
        /// </returns>
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        /// <summary>
        /// Методот <c>LinkLogin</c>
        /// се справува со POST барањата на патека /Manage/LinkLogin
        /// </summary>
        /// <param name="provider">Аргумент од тип string, кој претставува надворешен провајдер</param>
        /// <returns>
        /// Враќа објект од типот AccountController
        /// </returns>
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        /// <summary>
        /// Методот <c>LinkLoginCallback</c>
        /// се справува со GET барањата на патека /Manage/LinkLoginCallback
        /// </summary>
        /// <returns>
        /// Враќа соодветна редирекција кон /Manage/ManageLogins
        /// </returns>
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}