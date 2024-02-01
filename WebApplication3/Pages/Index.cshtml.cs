using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> signInManager;

        public string DecryptedCreditCard { get; private set; }

        public IndexModel(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            this.signInManager = signInManager;
        }
        public string UserId { get; set; }
        public string AuthToken { get; set; }
        public ApplicationUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            var session = _httpContextAccessor.HttpContext.Session;
            var cookieAuth = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            UserId = session.GetString("UserId");
            AuthToken = session.GetString("AuthToken");

            if (user == null)
            {
                return Page();
            }
             // Set the CurrentUser property to the retrieved user
            CurrentUser = user;
            if (CurrentUser != null || CurrentUser.AuthToken != null || cookieAuth != null)
            {
                if (cookieAuth != CurrentUser.AuthToken)

                {
                    user.AuthToken = null;
                    await _userManager.UpdateAsync(user);
                    await signInManager.SignOutAsync();

                    return RedirectToPage("/Login");
                }
            }
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            DecryptedCreditCard = protector.Unprotect(CurrentUser.CreditCard);

            return Page();
        }
    }
}
