using Bugtracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bugtracker.Models.Account;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Bugtracker.Models.Common.Status;
using Bugtracker.Models.Common.Enum;

namespace Bugtracker.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 IUserStore<ApplicationUser> userStore,
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IEmailSender emailSender,
                                 ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        #region Login

        [HttpGet]
        public async Task<IActionResult> Login(StatusBO? status)
        {
            var account = new AccountDTO();

            if (!string.IsNullOrEmpty(status?.StatusMessage))
            {
                account.Status = SetStatusDetails(status.StatusType);
            }
            else
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginBO login)
        {
            var account = new AccountDTO { Login = login };

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    account.Status = SetStatusDetails(Status.Error);

                    return RedirectToAction("Login", "Account", account.Status);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(account);
        }

        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterBO());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterBO register)
        {
            var account = new AccountDTO
            {
                Register = register
            };

            IdentityResult? result;

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, register.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, register.Email, CancellationToken.None);
                result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return View("RegisterConfirmation.cshtml");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                account.Status = SetStatusDetails(Status.Error);
                return View(account);
            }
            return View(account);
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Settings

        [HttpGet]
        public async Task<IActionResult> Settings(SettingType settingType = SettingType.Details)
        {

            var user = await _userManager.GetUserAsync(User);
            var account = new AccountDTO();
            account.Settings.UserName = user.UserName;
            account.Settings.Email = user.Email;
            account.Settings.SettingType = settingType;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsBO settings)
        {
            AccountDTO account = new AccountDTO
            {
                Settings = settings
            };

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError("", $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(account);
            }

            await _signInManager.RefreshSignInAsync(user);
            account.Status = SetStatusDetails(Status.Success);
            return View(new AccountDTO());
        }

        #endregion

        #region UsageMethods

        private StatusBO SetStatusDetails(Status status)
        {
            var StatusBO = new StatusBO();

            switch (status)
            {
                case Status.Success:
                    StatusBO.StatusMessage = "Action successful";
                    StatusBO.StatusType = Status.Success;
                    break;
                case Status.Error:
                    StatusBO.StatusMessage = "Error from Action";
                    StatusBO.StatusType = Status.Error;
                    break;
                default:
                    break;
            }

            return StatusBO;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        #endregion

        #region ChangePassword

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsBO settings)
        {
            var account = new AccountDTO
            {
                Settings = settings
            };

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                account.Status = SetStatusDetails(Status.Error);
                return View("Settings", account);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, settings.OldPassword, settings.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                account.Status = SetStatusDetails(Status.Error);
                return View("Settings", account);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");

            account.Status = SetStatusDetails(Status.Success);
            return View("Settings", account);
        }

        #endregion

        #region ChangeEmail

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(SettingsBO settings)
        {
            var account = new AccountDTO
            {
                Settings = settings
            };

            account.Settings.SettingType = SettingType.Email;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (ModelState.IsValid)
            {
                var email = await _userManager.GetEmailAsync(user);
                if (settings.NewEmail != email)
                {
                    account.Status = SetStatusDetails(Status.Success);
                    return View("Settings", account);
                }
            }

            account.Status = SetStatusDetails(Status.Error);
            return View("Settings", account);
        }

        #endregion
    }
}
