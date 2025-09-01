using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        // Password change fields - optional
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; } // Made nullable for clarity

        [DataType(DataType.Password)]
        // Removed [MinLength(6)]. Identity's UserManager will handle password policy.
        public string? NewPassword { get; set; } // Made nullable

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; } // Made nullable
    }
}