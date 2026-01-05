using System.ComponentModel.DataAnnotations;

namespace CourseDx.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "Username or Email")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool Remember { get; set; }
        // This will be populated from the hidden form field
        //public string ReturnUrl { get; set; }
    }
}