using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models.Admin
{
    public class UpdateUserViewModel
    {
        public string Id { get; set; }

        public string ConcurrencyStamp { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Leave password optional; when blank we will keep the existing password
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}
