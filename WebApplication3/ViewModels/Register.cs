using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication3.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Credit Card No is required.")]
        [RegularExpression(@"^\d{12,19}$", ErrorMessage = "Invalid Credit Card Number.")]
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(GenderType), ErrorMessage = "Invalid Gender.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Mobile No is required.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Invalid Mobile Number. Please enter a 8-digit number.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Delivery Address is required.")]
        [StringLength(255, ErrorMessage = "Delivery Address cannot exceed 255 characters.")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%#?&])[A-Za-z\d@$!%*#?&]+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg" }, ErrorMessage = "Only JPG files are allowed.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum file size allowed is 5 MB.")]
        public IFormFile Photo { get; set; }

        [StringLength(500, ErrorMessage = "About Me cannot exceed 500 characters.")]
        public string AboutMe { get; set; }
    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                return _extensions.Contains(extension);
            }
            return false;
        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;

        public MaxFileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                return file.Length <= _maxFileSize;
            }
            return false;
        }
    }
}
