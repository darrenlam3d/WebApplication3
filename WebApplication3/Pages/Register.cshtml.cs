using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                // Check if a user with the same email already exists
                var existingUser = await userManager.FindByEmailAsync(RModel.Email);

                if (existingUser != null)
                {
                    // A user with the same email already exists
                    ModelState.AddModelError("", "A user with this email already exists.");
                    return Page();
                }

                string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%#?&])[A-Za-z\d@$!%*#?&]+$";

                if (!Regex.IsMatch(RModel.Password, passwordPattern))
                {
                    ModelState.AddModelError("RModel.Password", "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                    return Page();
                }

                var user = new ApplicationUser()
                {
                    UserName = HttpUtility.HtmlEncode(RModel.Email),
                    FullName = HttpUtility.HtmlEncode(RModel.FullName),
                    Email = HttpUtility.HtmlEncode(RModel.Email),
                    CreditCard = protector.Protect(HttpUtility.HtmlEncode(RModel.CreditCard)),
                    Gender = HttpUtility.HtmlEncode(RModel.Gender),
                    MobileNo = HttpUtility.HtmlEncode(RModel.MobileNo),
                    DeliveryAddress = HttpUtility.HtmlEncode(RModel.DeliveryAddress),
                    AboutMe = HttpUtility.HtmlEncode(RModel.AboutMe),
                };

                var result = await userManager.CreateAsync(user, HttpUtility.HtmlEncode(RModel.Password));

                await HandlePhotoUpload(RModel.Photo, user.Email);

                if (RModel.Password != RModel.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Password and confirmation password do not match.");
                    return Page();
                }

                if (result.Succeeded)
                {
                    
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return Page();
            }

            return Page();
        }
        private async Task HandlePhotoUpload(IFormFile photo, string email)
        {
            if (photo != null && photo.Length > 0)
            {
                // Define the directory where photos will be stored
                var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Create a unique name for the file
                var fileName = $"{email}_{DateTime.UtcNow:yyyyMMddHHmmss}_{photo.FileName}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                // Update the user record with the file path
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.Photo = fileName;
                    await userManager.UpdateAsync(user);
                }
            }
        }
    }
}

