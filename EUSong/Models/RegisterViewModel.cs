// Models/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace EUSong.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
