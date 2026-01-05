using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.Admin
{
    public class UpdateUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string ConcurrencyStamp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Leave password optional; when blank we will keep the existing password
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}
